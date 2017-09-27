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

namespace TellMe.DAL.Types.Services
{
    public class StoryService : IStoryService
    {
        private readonly IRepository<Story, int> _storyRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        public StoryService(
            IRepository<Story, int> storyRepository,
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService)
        {
            _storyRepository = storyRepository;
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
        }

        public async Task RequestStoryAsync(string userId, StoryRequestDTO dto)
        {
            var user = await _userRepository.GetQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            var entity = Mapper.Map<Story>(dto);
            entity.SenderId = userId;
            entity.RequestDateUtc = DateTime.UtcNow;
            entity.Status = StoryStatus.Requested;
            _storyRepository.Save(entity, true);

            var storyDTO = Mapper.Map<StoryDTO>(entity);
            await _pushNotificationsService.SendStoryRequestPushNotificationAsync(storyDTO, user.UserName).ConfigureAwait(false);
        }
    }
}