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

namespace TellMe.DAL.Types.Services
{
    public class StoryService : IStoryService
    {
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<Contact, int> _contactRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;

        private readonly IPushNotificationsService _pushNotificationsService;
        public StoryService(
            IRepository<Story, int> storyRepository,
            IRepository<Contact, int> contactRepository,
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService)
        {
            _storyRepository = storyRepository;
            _contactRepository = contactRepository;
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
        }

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, string userId = null)
        {
            var stories = _storyRepository
                            .GetQueryable()
                            .AsNoTracking();

            if (userId != null)
            {
                stories = stories
                .Where(x => (x.SenderId == currentUserId && x.ReceiverId == userId) || (x.SenderId == userId && x.ReceiverId == currentUserId));
            }
            else
            {
                stories = stories.Where(x => x.SenderId == currentUserId || x.ReceiverId == currentUserId);
            }

            var result = await stories.ProjectTo<StoryDTO>()
            .ToListAsync()
            .ConfigureAwait(false);

            return result;
        }

        public async Task RequestStoryAsync(string requestSenderId, StoryRequestDTO dto)
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
            var storyDTO = Mapper.Map<StoryDTO>(entity);

            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == requestSenderId)
            .ConfigureAwait(false);

            var contact = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                    x.UserId == dto.ReceiverId
                    && x.PhoneNumberDigits == user.PhoneNumberDigits)
            .ConfigureAwait(false);
            string senderName = contact?.Name ?? user.PhoneNumber;

            await _pushNotificationsService.SendStoryRequestPushNotificationAsync(storyDTO, senderName).ConfigureAwait(false);
        }

        public async Task<ICollection<StoryDTO>> SendStoryAsync(string senderId, SendStoryDTO dto)
        {
            var now = DateTime.UtcNow;
            var entities = new List<Story>();
            if (dto.Id.HasValue)
            {
                var entity = await _storyRepository.GetQueryable().FirstOrDefaultAsync(x=>x.Id == dto.Id).ConfigureAwait(false);
                entity.VideoUrl = dto.VideoUrl;
                entity.PreviewUrl = dto.PreviewUrl;
                entity.Status = StoryStatus.Sent;
                entity.UpdateDateUtc = now;
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

            var storyDTOs = Mapper.Map<List<StoryDTO>>(entities);

            await _pushNotificationsService.SendStoryPushNotificationAsync(storyDTOs, senderId).ConfigureAwait(false);

            return storyDTOs;
        }
    }
}