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
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        public StoryService(
            IRepository<Story, int> storyRepository,
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Notification, int> notificationRepository)
        {
            _storyRepository = storyRepository;
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int? skip = null)
        {
            IQueryable<Story> stories = _storyRepository
                            .GetQueryable()
                            .AsNoTracking()
                            .Include(x => x.Sender)
                            .Include(x => x.Receiver)
                            .Where(x =>
                            (x.SenderId == currentUserId && x.Status == StoryStatus.Sent)
                            || (x.ReceiverId == currentUserId && x.Status == StoryStatus.Sent))
                            .OrderByDescending(x => x.CreateDateUtc);
            if (skip.HasValue)
            {
                stories = stories.Skip(skip.Value).Take(20);
            }

            var result = await stories
            .ProjectTo<StoryDTO>()
            .ToListAsync()
            .ConfigureAwait(false);

            return result;
        }

        public async Task<StoryStatus> RejectRequestAsync(string currentUserId, int storyId)
        {
            var story = await _storyRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == storyId && x.SenderId == currentUserId).ConfigureAwait(false);
            if (story.Status == StoryStatus.Requested)
            {
                story.Status = StoryStatus.Ignored;
            }

            await _storyRepository.SaveAsync(story, true).ConfigureAwait(false);

            return story.Status;
        }

        public async Task<StoryDTO> RequestStoryAsync(string requestSenderId, StoryRequestDTO dto)
        {
            var now = DateTime.UtcNow;
            var entity = new Story()
            {
                RequestDateUtc = now,
                UpdateDateUtc = now,
                Title = dto.Title,
                SenderId = dto.ReceiverId, // story sender is request receiver
                ReceiverId = requestSenderId, // story receiver is request sender
                Status = StoryStatus.Requested,
            };

            _storyRepository.Save(entity, true);

            entity = await _storyRepository.GetQueryable(true).Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);
            var storyDTO = Mapper.Map<StoryDTO>(entity);

            var notification = new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.StoryRequest,
                RecipientId = storyDTO.SenderId,
                Extra = storyDTO,
                Text = $"{storyDTO.ReceiverName} would like you to tell a story about {storyDTO.Title}"
            };

            await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);
            return storyDTO;
        }

        public async Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto)
        {
            var now = DateTime.UtcNow;
            Story entity;
            if (dto.Id.HasValue)
            {
                entity = await _storyRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id).ConfigureAwait(false);
                entity.VideoUrl = dto.VideoUrl;
                entity.PreviewUrl = dto.PreviewUrl;
                entity.Status = StoryStatus.Sent;
                entity.UpdateDateUtc = now;
                entity.CreateDateUtc = now;
            }
            else
            {
                entity = new Story()
                {
                    CreateDateUtc = now,
                    UpdateDateUtc = now,
                    Title = dto.Title,
                    SenderId = senderId,
                    ReceiverId = dto.ReceiverId,
                    Status = StoryStatus.Sent,
                    VideoUrl = dto.VideoUrl,
                    PreviewUrl = dto.PreviewUrl
                };
            }

            _storyRepository.Save(entity, true);
            entity = await _storyRepository.GetQueryable(true).Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);

            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == senderId)
            .ConfigureAwait(false);

            var storyDTO = Mapper.Map<StoryDTO>(entity);
            var notification = new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.Story,
                RecipientId = storyDTO.ReceiverId,
                Extra = storyDTO,
                Text = $"{user.UserName} has shared with you a story about {storyDTO.Title}"
            };

            await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);
            return storyDTO;
        }
    }
}