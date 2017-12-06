using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TellMe.Web.DAL.Contracts;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Types.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Comment, int> _commentRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<StoryReceiver, int> _storyReceiverRepository;

        private readonly IRepository<TribeMember, int> _tribeMemberRepository;

        public CommentService(IRepository<Comment, int> commentRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Story, int> storyRepository,
            IRepository<TribeMember, int> tribeMemberRepository,
            IRepository<StoryReceiver, int> storyReceiverRepository,
            IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _pushNotificationsService = pushNotificationsService;
            _storyRepository = storyRepository;
            _tribeMemberRepository = tribeMemberRepository;
            _storyReceiverRepository = storyReceiverRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentDTO> AddCommentAsync(string currentUserId, int storyId, CommentDTO comment)
        {
            _unitOfWork.BeginTransaction();
            var now = DateTime.UtcNow;
            var entity = Mapper.Map<Comment>(comment);
            entity.CreateDateUtc = now;
            entity.AuthorId = currentUserId;
            entity.StoryId = storyId;
            await _commentRepository.SaveAsync(entity, true).ConfigureAwait(false);

            Comment parentComment = null;
            if (comment.ReplyToCommentId != null)
            {
                parentComment = await _commentRepository
                    .GetQueryable()
                    .Where(x => x.Id == comment.ReplyToCommentId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                parentComment.RepliesCount++;
                await _commentRepository.SaveAsync(parentComment, true).ConfigureAwait(false);
            }

            var entityId = entity.Id;
            entity = await _commentRepository.GetQueryable(true).Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == entityId).ConfigureAwait(false);
            Mapper.Map(entity, comment);

            var story = await _storyRepository
                .GetQueryable()
                .Include(x => x.Sender)
                .Where(x => x.Id == comment.StoryId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            story.CommentsCount++;
            await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);

            var tribeMembers = _tribeMemberRepository
                .GetQueryable(true)
                .Where(x => x.UserId != currentUserId && x.UserId != story.SenderId);
            var storyReceivers = _storyReceiverRepository
                .GetQueryable(true)
                .Include(x => x.Tribe)
                .Where(x => x.StoryId == comment.StoryId && x.UserId != currentUserId && x.UserId != story.SenderId);

            var receivers = await (from receiver in storyReceivers
                join tribeMember in tribeMembers
                    on receiver.TribeId equals tribeMember.TribeId into gj
                from tb in gj.DefaultIfEmpty()
                where
                    tb == null || (tb.Status == TribeMemberStatus.Joined || tb.Status == TribeMemberStatus.Creator)
                select new
                {
                    UserId = tb != null ? tb.UserId : receiver.UserId,
                    TribeName = tb != null ? receiver.Tribe.Name : null
                }).ToListAsync().ConfigureAwait(false);


            var notifications = receivers.Select(receiver => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.StoryComment,
                RecipientId = receiver.UserId,
                Extra = comment,
                Text = receiver.TribeName == null
                    ? $"{comment.AuthorUserName} commented a {story.Sender.UserName}'s story \"{story.Title}\""
                    : $"[{receiver.TribeName}]: {comment.AuthorUserName} commented a {story.Sender.UserName}'s story \"{story.Title}\""
            }).ToList();
            notifications.Add(new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.StoryComment,
                RecipientId = story.Sender.Id,
                Extra = comment,
                Text = $"{comment.AuthorUserName} commented your story \"{story.Title}\""
            });

            if (parentComment != null)
            {
                notifications.Add(new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.StoryCommentReply,
                    RecipientId = parentComment.AuthorId,
                    Extra = comment,
                    Text =
                        $"{comment.AuthorUserName} replied to your comment to the {story.Sender.UserName}'s story \"{story.Title}\""
                });
            }

            _unitOfWork.SaveChanges();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
            return comment;
        }

        public async Task DeleteCommentAsync(string currentUserId, int storyId, int commentId)
        {
            _unitOfWork.BeginTransaction();

            var comment = await _commentRepository
                .GetQueryable()
                .Include(x => x.ReplyToComment)
                .FirstOrDefaultAsync(x => x.Id == commentId && x.StoryId == storyId && x.AuthorId == currentUserId)
                .ConfigureAwait(false);

            if (comment.ReplyToComment != null)
            {
                comment.ReplyToComment.RepliesCount--;
                await _commentRepository.SaveAsync(comment.ReplyToComment, true).ConfigureAwait(false);
            }

            var story = await _storyRepository
                .GetQueryable()
                .Where(x => x.Id == comment.StoryId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            story.CommentsCount--;
            await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);


            if (comment == null)
            {
                throw new Exception("Comment was not found or you don't have permissions to delete it");
            }

            _commentRepository.Remove(comment, true);
            _unitOfWork.SaveChanges();
        }

        public async Task<BulkDTO<CommentDTO>> GetAllAsync(int storyId, string currentUserId, DateTime olderThanUtc,
            int? replyToCommentId = null)
        {
            var comments = await _commentRepository
                .GetQueryable(true)
                .Include(x => x.Author)
                .Where(x => x.StoryId == storyId && x.CreateDateUtc < olderThanUtc &&
                            x.ReplyToCommentId == replyToCommentId)
                .OrderByDescending(x => x.CreateDateUtc)
                .Take(5)
                .ProjectTo<CommentDTO>()
                .ToListAsync()
                .ConfigureAwait(false);

            var totalCount = await _commentRepository
                .GetQueryable(true)
                .CountAsync(x => x.StoryId == storyId)
                .ConfigureAwait(false);

            var result = new BulkDTO<CommentDTO>
            {
                Items = comments,
                TotalCount = totalCount,
                OlderThanUtc = olderThanUtc
            };

            return result;
        }
    }
}