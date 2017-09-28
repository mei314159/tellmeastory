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

        public async Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, string userId)
        {
            var stories = await _storyRepository
                            .GetQueryable()
                            .AsNoTracking()
                            .Where(x => (x.SenderId == currentUserId && x.ReceiverId == userId) || (x.SenderId == userId && x.ReceiverId == currentUserId))
                            .ProjectTo<StoryDTO>()
                            .ToListAsync()
                            .ConfigureAwait(false);

            return stories;
        }

        public async Task RequestStoryAsync(string senderId, StoryRequestDTO dto)
        {
            string senderName;
            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == senderId)
            .ConfigureAwait(false);

            var contact = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                    x.UserId == dto.ReceiverId
                    && x.PhoneNumberDigits == user.PhoneNumberDigits)
            .ConfigureAwait(false);

            senderName = contact?.Name ?? user.PhoneNumber;
            var entity = Mapper.Map<Story>(dto);
            entity.SenderId = senderId;
            entity.RequestDateUtc = DateTime.UtcNow;
            entity.Status = StoryStatus.Requested;
            _storyRepository.Save(entity, true);

            var storyDTO = Mapper.Map<StoryDTO>(entity);
            await _pushNotificationsService.SendStoryRequestPushNotificationAsync(storyDTO, senderName).ConfigureAwait(false);
        }
    }
}