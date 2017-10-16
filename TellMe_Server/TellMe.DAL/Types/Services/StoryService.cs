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

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId)
        {
            var stories = _storyRepository
                            .GetQueryable()
                            .AsNoTracking()
                            .Include(x => x.Sender)
                            .Include(x => x.Receiver)
                            .Where(x => x.SenderId == currentUserId || x.ReceiverId == currentUserId);
            var result = await stories
            .ProjectTo<StoryDTO>()
            .ToListAsync()
            .ConfigureAwait(false);

            return result;
        }

        public async Task<ICollection<StoryDTO>> RequestStoryAsync(string requestSenderId, StoryRequestDTO dto)
        {
            var now = DateTime.UtcNow;
            var entities = new List<Story>();

            foreach (var receiverId in dto.ReceiverIds)
            {
                var entity = new Story()
                {
                    RequestDateUtc = now,
                    UpdateDateUtc = now,
                    Title = dto.Title,
                    SenderId = receiverId, // story sender is request receiver
                    ReceiverId = requestSenderId, // story receiver is request sender
                    Status = StoryStatus.Requested,
                };

                entities.Add(entity);
                _storyRepository.Save(entity, false);
            }

            _storyRepository.PreCommitSave();
            var storyDTOs = Mapper.Map<List<StoryDTO>>(entities);

            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == requestSenderId)
            .ConfigureAwait(false);

            var notifications = storyDTOs.Select(x => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.StoryRequest,
                RecipientId = x.SenderId,
                Extra = x,
                Text = $"{user.UserName} requested a story: {x.Title}"
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return storyDTOs;
        }

        public async Task<ICollection<StoryDTO>> SendStoryAsync(string senderId, SendStoryDTO dto)
        {
            var now = DateTime.UtcNow;
            var entities = new List<Story>();
            if (dto.Id.HasValue)
            {
                var entity = await _storyRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id).ConfigureAwait(false);
                entity.VideoUrl = dto.VideoUrl;
                entity.PreviewUrl = dto.PreviewUrl;
                entity.Status = StoryStatus.Sent;
                entity.UpdateDateUtc = now;
                entity.CreateDateUtc = now;
                _storyRepository.Save(entity, false);
                entities.Add(entity);
            }
            else
            {
                foreach (var receiverId in dto.ReceiverIds)
                {
                    var entity = new Story()
                    {
                        CreateDateUtc = now,
                        UpdateDateUtc = now,
                        Title = dto.Title,
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Status = StoryStatus.Sent,
                        VideoUrl = dto.VideoUrl,
                        PreviewUrl = dto.PreviewUrl
                    };

                    entities.Add(entity);
                    _storyRepository.Save(entity, false);
                }

            }
            _storyRepository.PreCommitSave();
            
            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == senderId)
            .ConfigureAwait(false);

            var storyDTOs = Mapper.Map<List<StoryDTO>>(entities);
            var notifications = storyDTOs.Select(x => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.Story,
                RecipientId = x.ReceiverId,
                Extra = x,
                Text = $"{user.UserName} sent a story: {x.Title}"
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return storyDTOs;
        }
    }
}