using System.Threading.Tasks;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Contracts.DTO;
using System;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Contracts.PushNotification;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Services
{
    public class TribeService : ITribeService
    {
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Tribe, int> _tribeRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;


        public TribeService(
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Notification, int> notificationRepository,
            IRepository<Tribe, int> tribeRepository,
            IRepository<TribeMember, int> tribeMemberRepository)
        {
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
            _tribeRepository = tribeRepository;
            _tribeMemberRepository = tribeMemberRepository;
        }
        public async Task<TribeDTO> CreateAsync(string currentUserId, TribeDTO dto)
        {
            DateTime now = DateTime.UtcNow;
            var entity = new Tribe
            {
                Name = dto.Name,
                CreateDateUtc = now,
                CreatorId = currentUserId,
                Members = dto.Members.Select(x => new TribeMember
                {
                    UserId = x.Id,
                    Status = TribeMemberStatus.Invited
                }).ToList()
            };

            entity.Members.Add(new TribeMember { UserId = currentUserId, Status = TribeMemberStatus.Creator });
            await _tribeRepository.SaveAsync(entity, true);

            var creator = await _userRepository.GetQueryable(true).FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
            var result = Mapper.Map<TribeDTO>(entity);
            result.MembershipStatus = TribeMemberStatus.Creator;
            Mapper.Map<ApplicationUser, TribeDTO>(creator, result);
            var notifications = entity.Members.Where(x => x.Status != TribeMemberStatus.Creator).Select(x => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.TribeInvite,
                RecipientId = x.UserId,
                Extra = result,
                Text = $"{result.CreatorName} invited you to join a tribe \"{result.Name}\""
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);
            return result;
        }

        public async Task<TribeMemberStatus> RejectTribeInvitationAsync(string currentUserId, int tribeId)
        {
            var tribeMember = await _tribeMemberRepository
            .GetQueryable()
            .Include(x => x.User)
            .Include(x => x.Tribe)
            .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.TribeId == tribeId)
            .ConfigureAwait(false);

            tribeMember.Status = TribeMemberStatus.Rejected;
            _tribeMemberRepository.PreCommitSave();

            var creatorId = await _tribeMemberRepository
                .GetQueryable(true)
                .Where(x => x.TribeId == tribeId && x.Status == TribeMemberStatus.Creator)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var notification = new Notification
            {
                Date = DateTime.UtcNow,
                Type = NotificationTypeEnum.TribeRejectInvite,
                RecipientId = creatorId,
                Extra = Mapper.Map<TribeMemberDTO>(tribeMember),
                Text = $"[{tribeMember.Tribe.Name}]: {tribeMember.User.UserName} rejected your invitation to the tribe"
            };

            await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);

            return TribeMemberStatus.Rejected;
        }

        public async Task<TribeMemberStatus> AcceptTribeInvitationAsync(string currentUserId, int tribeId)
        {
            var tribe = await _tribeRepository
            .GetQueryable()
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == tribeId)
            .ConfigureAwait(false);

            var tribeMember = tribe.Members.FirstOrDefault(x => x.UserId == currentUserId);
            tribeMember.Status = TribeMemberStatus.Joined;
            await _tribeMemberRepository.SaveAsync(tribeMember, true).ConfigureAwait(false);

            var tribeMemberDTO = Mapper.Map<TribeMemberDTO>(tribeMember);
            var notifications = tribe.Members
            .Where(x => x.UserId != currentUserId && x.Status == TribeMemberStatus.Joined || x.Status == TribeMemberStatus.Creator)
            .ToList()
            .Select(x => new Notification
            {
                Date = DateTime.UtcNow,
                Type = NotificationTypeEnum.TribeRejectInvite,
                RecipientId = x.UserId,
                Extra = tribeMemberDTO,
                Text = $"[{tribeMember.Tribe.Name}]: {tribeMember.User.UserName} joined the tribe"
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            await _pushNotificationsService.SendPushNotificationAsync(notifications).ConfigureAwait(false);

            return TribeMemberStatus.Joined;
        }
    }
}