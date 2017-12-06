using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TellMe.Shared.Contracts.DTO;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Types.Services
{
    public class StoryService : IStoryService
    {
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IPushNotificationsService _pushNotificationsService;

        private readonly IRepository<StoryRequestStatus, int> _storyRequestStatusRepository;
        private readonly IRepository<StoryRequest, int> _storyRequestRepository;
        private readonly IRepository<StoryReceiver, int> _storyReceiverRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        private readonly IRepository<StoryLike> _storyLikeRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<EventAttendee, int> _eventAttendeeRepository;
        private readonly IRepository<Event, int> _eventRepository;

        public StoryService(
            IRepository<Story, int> storyRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<StoryRequestStatus, int> storyRequestStatusRepository,
            IRepository<StoryRequest, int> storyRequestRepository,
            IRepository<StoryReceiver, int> storyReceiverRepository,
            IRepository<TribeMember, int> tribeMemberRepository,
            IRepository<StoryLike> storyLikeRepository,
            IRepository<ApplicationUser> userRepository,
            IRepository<EventAttendee, int> eventAttendeeRepository,
            IRepository<Event, int> eventRepository)
        {
            _storyRepository = storyRepository;
            _pushNotificationsService = pushNotificationsService;
            _storyRequestStatusRepository = storyRequestStatusRepository;
            _storyRequestRepository = storyRequestRepository;
            _storyReceiverRepository = storyReceiverRepository;
            _tribeMemberRepository = tribeMemberRepository;
            _storyLikeRepository = storyLikeRepository;
            _userRepository = userRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
            _eventRepository = eventRepository;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc)
        {
            var tribeMembers = _tribeMemberRepository.GetQueryable(true).Where(x => x.UserId == currentUserId);
            var receivers = _storyReceiverRepository.GetQueryable(true);

            receivers = receivers
                .GroupJoin(tribeMembers, receiver => receiver.TribeId, tribeMember => tribeMember.Tribe.Id,
                    (receiver, gj) => new {receiver, gj})
                .SelectMany(t => t.gj.DefaultIfEmpty(), (t, tb) => new {t, tb})
                .Where(t =>
                    t.t.receiver.UserId == currentUserId ||
                    (t.tb != null && (t.tb.Status == TribeMemberStatus.Joined ||
                                      t.tb.Status == TribeMemberStatus.Creator)))
                .Select(t => t.t.receiver);

            var eventAttendees = _eventAttendeeRepository.GetQueryable(true);
            eventAttendees = from attendee in eventAttendees
                join tribeMember in tribeMembers
                    on attendee.TribeId equals tribeMember.Tribe.Id into atm
                from tm in atm.DefaultIfEmpty()
                where (attendee.Status != EventAttendeeStatus.Rejected && attendee.UserId == currentUserId) ||
                      tm.UserId == currentUserId
                select attendee;

            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true);
            stories = stories
                .GroupJoin(receivers, story => story.Id, receiver => receiver.Story.Id,
                    (story, receiversGroup) => new {story, receiversGroup})
                .SelectMany(x => x.receiversGroup.DefaultIfEmpty(), (x, storyReceiver) => new {x.story, storyReceiver})
                .GroupJoin(eventAttendees, x => x.story.EventId, x => x.Event.Id,
                    (group, attendeesGroup) => new {group.story, group.storyReceiver, attendeesGroup})
                .SelectMany(x => x.attendeesGroup.DefaultIfEmpty(),
                    (x, attendee) => new {x.story, x.storyReceiver, attendee})
                .Where(x =>
                    x.story.CreateDateUtc < olderThanUtc &&
                    (x.story.SenderId == currentUserId || x.storyReceiver != null) &&
                    (x.story.Event == null || x.story.Event.HostId == currentUserId ||
                     (x.story.Event.ShareStories && x.attendee != null)))
                .Select(t => t.story)
                .Distinct()
                .OrderByDescending(t => t.CreateDateUtc)
                .Take(20)
                .Include(x => x.Event)
                .Include(x => x.Sender)
                .Include(x => x.Likes)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe);


            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list, x => x.Items["UserId"] = currentUserId);

            return result;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int eventId, DateTime olderThanUtc)
        {
            var sharedStories = await _eventRepository
                .GetQueryable(true)
                .AnyAsync(x => x.Id == eventId && (x.HostId == currentUserId || x.ShareStories))
                .ConfigureAwait(false);
            if (!sharedStories)
                return new StoryDTO[] { };

            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Likes)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe);
            stories = (from story in stories
                    where story.EventId == eventId && story.CreateDateUtc < olderThanUtc
                    orderby story.CreateDateUtc descending
                    select story)
                .Take(20);


            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list, x => x.Items["UserId"] = currentUserId);

            return result;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string userId, string currentUserId, DateTime olderThanUtc)
        {
            if (userId == currentUserId)
            {
                return await this.GetAllAsync(currentUserId, olderThanUtc).ConfigureAwait(false);
            }

            var receivers = _storyReceiverRepository.GetQueryable(true)
                .Where(receiver =>
                    receiver.UserId == currentUserId && receiver.Story.SenderId == userId ||
                    receiver.UserId == userId && receiver.Story.SenderId == currentUserId);

            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Likes)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe)
                .Where(t => t.CreateDateUtc < olderThanUtc);
            stories = stories
                .Join(receivers, story => story.Id, receiver => receiver.Story.Id, (story, gj) => story)
                .OrderByDescending(t => t.CreateDateUtc)
                .Take(20);
            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list, x => x.Items["UserId"] = currentUserId);

            return result;
        }

        public async Task<ICollection<StoryListDTO>> SearchAsync(string currentUserId, string fragment, int skip)
        {
            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true)
                .FromSql(new RawSqlString(@"SELECT story.*
            FROM [dbo].[Story] AS story 
            LEFT JOIN CONTAINSTABLE ([dbo].[Story], Title, @p0) AS sk
            ON story.Id = sk.[KEY]
            LEFT JOIN CONTAINSTABLE ([dbo].[Event], (Title, Description), @p0) AS ek
            ON story.EventId = ek.[KEY]
            LEFT JOIN CONTAINSTABLE ([dbo].[AspNetUsers], (FullName, UserName), @p0) AS uk
            ON story.SenderId = uk.[KEY]
            WHERE sk.RANK > 2 OR ek.RANK > 2 OR uk.RANK > 2
            ORDER BY sk.RANK DESC, ek.RANK DESC, uk.RANK DESC
            OFFSET @p1 ROWS
            FETCH NEXT @p2 ROWS ONLY"), fragment, skip, 20)
                .Include(x => x.Sender);

            var result = await stories.ProjectTo<StoryListDTO>().ToListAsync()
                .ConfigureAwait(false);
            return result;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(int tribeId, string currentUserId, DateTime olderThanUtc)
        {
            var tribeMember = await _tribeMemberRepository
                .GetQueryable(true)
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUserId && x.TribeId == tribeId &&
                    (x.Status == TribeMemberStatus.Joined || x.Status == TribeMemberStatus.Creator))
                .ConfigureAwait(false);

            if (tribeMember == null)
            {
                throw new Exception("Tribe doesn't exist or you're not a member of the tribe");
            }

            var receivers = _storyReceiverRepository
                .GetQueryable(true
                ).Where(x => x.TribeId == tribeId);

            var eventAttendees = _eventAttendeeRepository.GetQueryable(true)
                .Where(x => x.TribeId == tribeId);

            IQueryable<Story> stories = _storyRepository.GetQueryable(true)
                .Join(receivers, story => story.Id, receiver => receiver.Story.Id, (story, gj) => story)
                .GroupJoin(eventAttendees, x => x.EventId, x => x.Event.Id,
                    (story, attendeesGroup) => new {story, attendeesGroup})
                .SelectMany(x => x.attendeesGroup.DefaultIfEmpty(),
                    (x, attendee) => new {x.story, attendee})
                .Where(x => x.story.CreateDateUtc < olderThanUtc &&
                            (x.story.Event == null || x.story.Event.HostId == currentUserId ||
                             x.story.Event.ShareStories && x.attendee != null))
                .Select(x => x.story)
                .Distinct()
                .OrderByDescending(t => t.CreateDateUtc)
                .Take(20)
                .Include(x => x.Event)
                .Include(x => x.Sender)
                .Include(x => x.Likes)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe);


            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list, x => x.Items["UserId"] = currentUserId);

            return result;
        }

        public async Task<ICollection<StoryReceiverDTO>> GetStoryReceiversAsync(string currentUserId, int storyId)
        {
            var result = await _storyReceiverRepository
                .GetQueryable(true)
                .Include(x => x.User)
                .Include(x => x.Tribe)
                .Where(x => x.StoryId == storyId)
                .ProjectTo<StoryReceiverDTO>()
                .ToListAsync()
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StoryStatus> RejectRequestAsync(string currentUserId, int requestId)
        {
            var result = await SetRequestStatus(currentUserId, requestId, StoryStatus.Ignored).ConfigureAwait(false);
            return result;
        }

        public async Task<bool> LikeAsync(string currentUserId, int storyId)
        {
            var liked = await _storyLikeRepository.GetQueryable(true)
                .AnyAsync(x => x.StoryId == storyId && x.UserId == currentUserId)
                .ConfigureAwait(false);
            if (!liked)
            {
                await _storyLikeRepository.SaveAsync(new StoryLike
                {
                    UserId = currentUserId,
                    StoryId = storyId
                }, true).ConfigureAwait(false);

                var story = await _storyRepository
                    .GetQueryable()
                    .Include(x => x.Sender)
                    .FirstOrDefaultAsync(x => x.Id == storyId)
                    .ConfigureAwait(false);

                story.LikesCount++;
                await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);

                var username = await _userRepository
                    .GetQueryable(true)
                    .Where(x => x.Id == currentUserId)
                    .Select(x => x.UserName)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                var notification = new Notification
                {
                    Date = DateTime.UtcNow,
                    Type = NotificationTypeEnum.StoryLike,
                    RecipientId = story.Sender.Id,
                    Extra = new object(),
                    Text = $"{username} likes your story \"{story.Title}\""
                };

                await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);
            }

            return true;
        }

        public async Task<bool> DislikeAsync(string userId, int storyId)
        {
            var like = await _storyLikeRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.StoryId == storyId && x.UserId == userId)
                .ConfigureAwait(false);
            if (like != null)
            {
                _storyLikeRepository.Remove(like, true);
                var story = await _storyRepository
                    .GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == storyId)
                    .ConfigureAwait(false);
                story.LikesCount--;
                await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);
            }

            return true;
        }

        public async Task<ICollection<StoryRequestDTO>> RequestStoryAsync(string requestSenderId,
            IEnumerable<StoryRequestDTO> requests)
        {
            var entities = Mapper.Map<List<StoryRequest>>(requests);
            entities.ForEach(x => x.SenderId = requestSenderId);
            await _storyRequestRepository.AddRangeAsync(entities, true).ConfigureAwait(false);

            var tribeIds = requests.Select(x => x.TribeId).Where(x => x != null).ToList();
            var tribeMembers = await _tribeMemberRepository.GetQueryable(true).Where(x => tribeIds.Contains(x.TribeId))
                .Select(x => new
                {
                    x.TribeId,
                    TribeName = x.Tribe.Name,
                    x.UserId
                }).ToListAsync().ConfigureAwait(false);

            var now = DateTime.UtcNow;

            var entityIds = entities.Select(x => x.Id).ToArray();
            var requestDtos = await _storyRequestRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x => entityIds.Contains(x.Id))
                .ProjectTo<StoryRequestDTO>()
                .ToListAsync()
                .ConfigureAwait(false);

            var notifications = requestDtos.SelectMany(requestDTO =>
            {
                if (requestDTO.TribeId == null)
                {
                    return new[]
                    {
                        new Notification
                        {
                            Date = now,
                            Type = NotificationTypeEnum.StoryRequest,
                            RecipientId = requestDTO.UserId,
                            Extra = requestDTO,
                            Text = $"{requestDTO.SenderName} would like you to tell a story about {requestDTO.Title}"
                        }
                    };
                }

                return tribeMembers.Where(x => x.TribeId == requestDTO.TribeId && x.UserId != requestSenderId).Select(
                    x => new Notification
                    {
                        Date = now,
                        Type = NotificationTypeEnum.StoryRequest,
                        RecipientId = x.UserId,
                        Extra = requestDTO,
                        Text =
                            $"[{x.TribeName}]: {requestDTO.SenderName} would like you to tell a story about \"{requestDTO.Title}\""
                    });
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
            return requestDtos;
        }

        public async Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto)
        {
            var now = DateTime.UtcNow;

            var entity = new Story
            {
                CreateDateUtc = now,
                Title = dto.Title,
                SenderId = senderId,
                VideoUrl = dto.VideoUrl,
                PreviewUrl = dto.PreviewUrl,
                RequestId = dto.RequestId
            };

            if (dto.RequestId.HasValue)
            {
                var request = await _storyRequestRepository.GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == dto.RequestId).ConfigureAwait(false);
                entity.EventId = request.EventId;
                entity.Receivers = new[]
                {
                    new StoryReceiver
                    {
                        UserId = request.TribeId == null ? request.SenderId : null,
                        TribeId = request.TribeId
                    }
                };
            }
            else if (dto.Receivers?.Count > 0)
            {
                entity.Receivers = dto.Receivers.Select(x => new StoryReceiver
                {
                    UserId = x.TribeId == null ? x.UserId : null,
                    TribeId = x.TribeId
                }).ToArray();
            }

            _storyRepository.Save(entity, true);
            if (dto.RequestId.HasValue)
            {
                await SetRequestStatus(senderId, dto.RequestId.Value, StoryStatus.Sent).ConfigureAwait(false);
            }

            var entityId = entity.Id;
            entity = await _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Receivers)
                .FirstOrDefaultAsync(x => x.Id == entityId)
                .ConfigureAwait(false);

            var storyDTO = Mapper.Map<StoryDTO>(entity);

            var tribeIds = storyDTO.Receivers.Where(x => x.TribeId != null).Select(x => x.TribeId).ToList();
            var tribeMembers = await _tribeMemberRepository.GetQueryable(true).Where(x => tribeIds.Contains(x.TribeId))
                .Select(x => new
                {
                    x.TribeId,
                    TribeName = x.Tribe.Name,
                    x.UserId
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var notifications = storyDTO.Receivers.SelectMany(receiver =>
            {
                if (receiver.TribeId == null)
                {
                    return new[]
                    {
                        new Notification
                        {
                            Date = now,
                            Type = NotificationTypeEnum.Story,
                            RecipientId = receiver.UserId,
                            Extra = storyDTO,
                            Text = $"{storyDTO.SenderName} has shared with you a story about \"{storyDTO.Title}\""
                        }
                    };
                }

                return tribeMembers.Where(x => x.TribeId == receiver.TribeId).Select(x => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.Story,
                    RecipientId = x.UserId,
                    Extra = storyDTO,
                    Text =
                        $"[{x.TribeName}]: {storyDTO.SenderName} has shared with you a story about \"{storyDTO.Title}\""
                });
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
            return storyDTO;
        }

        private async Task<StoryStatus> SetRequestStatus(string currentUserId, int requestId, StoryStatus status)
        {
            var requestStatus = await _storyRequestStatusRepository
                                    .GetQueryable()
                                    .FirstOrDefaultAsync(x => x.RequestId == requestId && x.UserId == currentUserId)
                                    .ConfigureAwait(false) ?? new StoryRequestStatus
                                {
                                    RequestId = requestId,
                                    UserId = currentUserId,
                                    Status = status
                                };

            await _storyRequestStatusRepository.SaveAsync(requestStatus, true).ConfigureAwait(false);

            return requestStatus.Status;
        }
    }
}