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

            var result = await stories
            .ProjectTo<StoryDTO>()
            .ToListAsync()
            .ConfigureAwait(false);

            var ids = result
                    .Select(x => x.SenderId)
                    .Union(result.Select(x => x.ReceiverId)).ToArray();
            var users = _userRepository.GetQueryable().AsNoTracking().Where(x => ids.Contains(x.Id));

            var contacts = _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == currentUserId);

            var nm = await (from user in users
                            join contact in contacts
                            on user.PhoneNumberDigits
                            equals contact.PhoneNumberDigits into gj
                            from x in gj.DefaultIfEmpty()
                            select new { Name = x != null ? x.Name : user.PhoneNumber, user.Id })
            .ToDictionaryAsync(x => x.Id, x => x.Name).ConfigureAwait(false);
            foreach (var item in result)
            {
                item.SenderName = nm[item.SenderId];
                item.ReceiverName = nm[item.ReceiverId];
            }

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

            
            await _pushNotificationsService.SendStoryPushNotificationAsync(storyDTOs, requestSenderId).ConfigureAwait(false);

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
            var storyDTOs = Mapper.Map<List<StoryDTO>>(entities);

            await _pushNotificationsService.SendStoryPushNotificationAsync(storyDTOs, senderId).ConfigureAwait(false);

            return storyDTOs;
        }
    }
}