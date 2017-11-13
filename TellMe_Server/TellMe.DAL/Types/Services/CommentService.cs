using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.DTO;
using AutoMapper;
using TellMe.DAL.Contracts.PushNotification;
using System;
using AutoMapper.QueryableExtensions;
using TellMe.DAL.Types.PushNotifications;
using TellMe.DAL.Contracts;

namespace TellMe.DAL.Types.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Comment, int> _commentRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<StoryReceiver, int> _storyReceiverRepository;

        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        public CommentService(IRepository<Comment, int> commentRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Notification, int> notificationRepository,
            IRepository<Story, int> storyRepository,
            IRepository<TribeMember, int> tribeMemberRepository,
            IRepository<StoryReceiver, int> storyReceiverRepository,
            IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
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

            entity = await _commentRepository.GetQueryable(true).Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);
            Mapper.Map(entity, comment);

            var story = await _storyRepository
            .GetQueryable()
            .Include(x => x.Sender)
            .Where(x => x.Id == comment.StoryId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
            story.CommentsCount++;
            await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);

            var tribeMembers = _tribeMemberRepository
            .GetQueryable(true).Where(x => x.UserId != currentUserId);
            var storyReceivers = _storyReceiverRepository
            .GetQueryable(true)
            .Include(x => x.Tribe)
            .Where(x => x.StoryId == comment.StoryId && x.UserId != currentUserId);

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
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            _unitOfWork.SaveChanges();

            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return comment;
        }

        public async Task DeleteCommentAsync(string currentUserId, int storyId, int commentId)
        {
            _unitOfWork.BeginTransaction();

            var comment = await _commentRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == commentId && x.StoryId == storyId && x.AuthorId == currentUserId)
            .ConfigureAwait(false);

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

        public async Task<BulkDTO<CommentDTO>> GetAllAsync(int storyId, string currentUserId, DateTime olderThanUtc)
        {
            var comments = await _commentRepository
            .GetQueryable(true)
            .Include(x => x.Author)
            .Where(x => x.StoryId == storyId && x.CreateDateUtc < olderThanUtc)
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