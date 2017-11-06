using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.DTO;
using System.Text.RegularExpressions;
using AutoMapper;
using TellMe.DAL.Contracts.PushNotification;
using System;
using AutoMapper.QueryableExtensions;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Services
{
    public class StoryService : IStoryService
    {
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Notification, int> _notificationRepository;

        private readonly IRepository<StoryRequestStatus, int> _storyRequestStatusRepository;
        private readonly IRepository<StoryRequest, int> _storyRequestRepository;
        private readonly IRepository<StoryReceiver, int> _storyReceiverRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        public StoryService(
            IRepository<Story, int> storyRepository,
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Notification, int> notificationRepository,
            IRepository<StoryRequestStatus, int> storyRequestStatusRepository,
            IRepository<StoryRequest, int> storyRequestRepository,
            IRepository<StoryReceiver, int> storyReceiverRepository,
            IRepository<TribeMember, int> tribeMemberRepository)
        {
            _storyRepository = storyRepository;
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
            _storyRequestStatusRepository = storyRequestStatusRepository;
            _storyRequestRepository = storyRequestRepository;
            _storyReceiverRepository = storyReceiverRepository;
            _tribeMemberRepository = tribeMemberRepository;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int skip)
        {
            var tribeMembers = _tribeMemberRepository.GetQueryable(true).Where(x => x.UserId == currentUserId);
            var receivers = _storyReceiverRepository.GetQueryable(true);

            receivers = from receiver in receivers
                        join tribeMember in tribeMembers
                        on receiver.TribeId equals tribeMember.TribeId into gj
                        from tb in gj.DefaultIfEmpty()
                        where
                            receiver.UserId == currentUserId || (tb != null && (tb.Status == TribeMemberStatus.Joined || tb.Status == TribeMemberStatus.Creator))
                        select
                          receiver;

            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe);
            stories = (from story in stories
                       join receiver in receivers
                       on story.Id equals receiver.StoryId into gj
                       from st in gj.DefaultIfEmpty()
                       where story.SenderId == currentUserId || st != null
                       orderby story.CreateDateUtc descending
                       select story)
                       .Skip(skip)
                       .Take(20);


            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list);

            return result;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string userId, string currentUserId, int skip)
        {
            if (userId == currentUserId)
            {
                return await this.GetAllAsync(currentUserId, skip).ConfigureAwait(false);
            }

            var receivers = _storyReceiverRepository.GetQueryable(true)
            .Include(x => x.Story)
            .ThenInclude(x => x.Sender)
            .Where(receiver =>
                            (receiver.UserId == currentUserId && receiver.Story.SenderId == userId) ||
                            (receiver.UserId == userId && receiver.Story.SenderId == currentUserId));

            IQueryable<Story> stories = _storyRepository
             .GetQueryable(true)
             .Include(x => x.Sender)
             .Include(x => x.Receivers).ThenInclude(x => x.User)
             .Include(x => x.Receivers).ThenInclude(x => x.Tribe);
            stories = (from story in stories
                       join receiver in receivers
                       on story.Id equals receiver.StoryId into gj
                       from st in gj.DefaultIfEmpty()
                       where st != null
                       orderby story.CreateDateUtc descending
                       select story)
                       .Skip(skip)
                       .Take(20);
            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list);

            return result;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(int tribeId, string currentUserId, int skip)
        {
            var tribeMember = await _tribeMemberRepository
            .GetQueryable(true)
            .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.TribeId == tribeId && (x.Status == TribeMemberStatus.Joined || x.Status == TribeMemberStatus.Creator))
            .ConfigureAwait(false);

            if (tribeMember == null)
            {
                throw new Exception("Tribe is not exist or you're not a member of the tribe");
            }

            var receivers = _storyReceiverRepository
            .GetQueryable(true
            ).Where(x => x.TribeId == tribeId);

            IQueryable<Story> stories = _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe);
            stories = (from story in stories
                       join receiver in receivers
                       on story.Id equals receiver.StoryId into gj
                       from st in gj.DefaultIfEmpty()
                       where st != null
                       orderby story.CreateDateUtc descending
                       select story)
                       .Skip(skip)
                       .Take(20);


            var list = await stories.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<StoryDTO>>(list);

            return result;
        }

        public async Task<StoryStatus> RejectRequestAsync(string currentUserId, int requestId)
        {
            var result = await SetRequestStatus(currentUserId, requestId, StoryStatus.Ignored).ConfigureAwait(false);
            return result;
        }

        public async Task<ICollection<StoryRequestDTO>> RequestStoryAsync(string requestSenderId, IEnumerable<StoryRequestDTO> requests)
        {
            var entities = Mapper.Map<List<StoryRequest>>(requests);
            entities.ForEach(x => x.SenderId = requestSenderId);
            _storyRequestRepository.AddRange(entities, true);

            var tribeIds = requests.Select(x => x.TribeId).Where(x => x != null).ToList();
            var tribeMembers = await _tribeMemberRepository.GetQueryable(true).Where(x => tribeIds.Contains(x.TribeId))
            .Select(x => new
            {
                TribeId = x.TribeId,
                TribeName = x.Tribe.Name,
                x.UserId
            }).ToListAsync().ConfigureAwait(false);

            var now = DateTime.UtcNow;

            var entityIds = entities.Select(x => x.Id).ToArray();
            var requestDTOs = await _storyRequestRepository
            .GetQueryable(true)
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Where(x => entityIds.Contains(x.Id))
            .ProjectTo<StoryRequestDTO>()
            .ToListAsync()
            .ConfigureAwait(false);

            var notifications = requestDTOs.SelectMany(requestDTO =>
            {
                if (requestDTO.TribeId == null)
                {
                    return new[] {new Notification
                    {
                        Date = now,
                        Type = NotificationTypeEnum.StoryRequest,
                        RecipientId = requestDTO.UserId,
                        Extra = requestDTO,
                        Text = $"{requestDTO.SenderName} would like you to tell a story about {requestDTO.Title}"
                    }};
                }

                return tribeMembers.Where(x => x.TribeId == requestDTO.TribeId && x.UserId != requestSenderId).Select(x => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.StoryRequest,
                    RecipientId = x.UserId,
                    Extra = requestDTO,
                    Text = $"[{x.TribeName}]: {requestDTO.SenderName} would like you to tell a story about \"{requestDTO.Title}\""
                });
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return requestDTOs;
        }

        public async Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto)
        {
            var now = DateTime.UtcNow;

            var entity = new Story()
            {
                CreateDateUtc = now,
                Title = dto.Title,
                SenderId = senderId,
                VideoUrl = dto.VideoUrl,
                PreviewUrl = dto.PreviewUrl,
                RequestId = dto.RequestId
            };

            StoryRequest request;
            if (dto.RequestId.HasValue)
            {
                request = await _storyRequestRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.RequestId).ConfigureAwait(false);
                entity.Receivers = new[] { new StoryReceiver
                {
                    UserId = request.TribeId == null ? request.SenderId : null,
                    TribeId = request.TribeId
                }};
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

            entity = await _storyRepository
            .GetQueryable(true)
            .Include(x => x.Sender)
            .Include(x => x.Receivers)
            .FirstOrDefaultAsync(x => x.Id == entity.Id)
            .ConfigureAwait(false);

            var storyDTO = Mapper.Map<StoryDTO>(entity);


            var tribeIds = storyDTO.Receivers.Where(x => x.TribeId != null).Select(x => x.TribeId).ToList();
            var tribeMembers = await _tribeMemberRepository.GetQueryable(true).Where(x => tribeIds.Contains(x.TribeId))
            .Select(x => new
            {
                TribeId = x.TribeId,
                TribeName = x.Tribe.Name,
                x.UserId
            })
            .ToListAsync()
            .ConfigureAwait(false);

            var notifications = storyDTO.Receivers.SelectMany(receiver =>
            {
                if (receiver.TribeId == null)
                {
                    return new[]{
                    new Notification
                    {
                        Date = now,
                        Type = NotificationTypeEnum.Story,
                        RecipientId = receiver.UserId,
                        Extra = storyDTO,
                        Text = $"{storyDTO.SenderName} has shared with you a story about \"{storyDTO.Title}\""
                    }};
                }

                return tribeMembers.Where(x => x.TribeId == receiver.TribeId).Select(x => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.Story,
                    RecipientId = x.UserId,
                    Extra = storyDTO,
                    Text = $"[{x.TribeName}]: {storyDTO.SenderName} has shared with you a story about \"{storyDTO.Title}\""
                });
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return storyDTO;
        }

        private async Task<StoryStatus> SetRequestStatus(string currentUserId, int requestId, StoryStatus status)
        {
            var requestStatus = await _storyRequestStatusRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.RequestId == requestId && x.UserId == currentUserId)
            .ConfigureAwait(false);
            if (requestStatus == null)
            {
                requestStatus = new StoryRequestStatus
                {
                    RequestId = requestId,
                    UserId = currentUserId,
                    Status = StoryStatus.Ignored
                };
            }

            await _storyRequestStatusRepository.SaveAsync(requestStatus, true).ConfigureAwait(false);

            return requestStatus.Status;
        }
    }
}