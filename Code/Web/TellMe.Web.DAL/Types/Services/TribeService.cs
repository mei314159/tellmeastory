using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TellMe.Shared.Contracts.DTO;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Types.Services
{
    public class TribeService : ITribeService
    {
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Tribe, int> _tribeRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;

        public TribeService(
            IRepository<ApplicationUser, string> userRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Tribe, int> tribeRepository,
            IRepository<TribeMember, int> tribeMemberRepository)
        {
            _userRepository = userRepository;
            _pushNotificationsService = pushNotificationsService;
            _tribeRepository = tribeRepository;
            _tribeMemberRepository = tribeMemberRepository;
        }
        public async Task<SharedTribeDTO> CreateAsync(string currentUserId, SharedTribeDTO dto)
        {
            DateTime now = DateTime.UtcNow;
            var entity = new Tribe
            {
                Name = dto.Name,
                CreateDateUtc = now,
                CreatorId = currentUserId,
                Members = dto.Members.Select(x => new TribeMember
                {
                    UserId = x.UserId,
                    Status = TribeMemberStatus.Invited
                }).ToList()
            };

            entity.Members.Add(new TribeMember { UserId = currentUserId, Status = TribeMemberStatus.Creator });
            await _tribeRepository.SaveAsync(entity, true);

            var creator = await _userRepository.GetQueryable(true).FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);

            var result = new SharedTribeDTO
            {
                MembershipStatus = TribeMemberStatus.Creator
            };
            Mapper.Map(entity, result);
            Mapper.Map(creator, result);
            await NotifyMembersAboutInvitationToTribeAsync(entity.Members.Where(x => x.Status != TribeMemberStatus.Creator), result).ConfigureAwait(false);
            result.Members = Mapper.Map<List<SharedTribeMemberDTO>>(entity.Members);
            return result;
        }

        public async Task LeaveTribeAsync(string currentUserId, int tribeId)
        {
            var tribe = await _tribeRepository
            .GetQueryable()
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == tribeId)
            .ConfigureAwait(false);

            var tribeMember = tribe.Members.FirstOrDefault(x => x.UserId == currentUserId);
            _tribeMemberRepository.Remove(tribeMember, true);
            tribe.Members.Remove(tribeMember);

            var tribeMemberDTO = Mapper.Map<SharedTribeMemberDTO>(tribeMember);
            var notifications = tribe.Members
            .Where(x => x.UserId != currentUserId && x.Status == TribeMemberStatus.Joined || x.Status == TribeMemberStatus.Creator)
            .ToList()
            .Select(x => new Notification
            {
                Date = DateTime.UtcNow,
                Type = NotificationTypeEnum.LeftTribe,
                RecipientId = x.UserId,
                Extra = tribeMemberDTO,
                Handled = true,
                Text = $"[{tribeMember.Tribe.Name}]: {tribeMember.User.UserName} has left the tribe"
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
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
                Extra = Mapper.Map<SharedTribeMemberDTO>(tribeMember),
                Text = $"[{tribeMember.Tribe.Name}]: {tribeMember.User.UserName} rejected your invitation to the tribe"
            };

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

            var tribeMemberDTO = Mapper.Map<SharedTribeMemberDTO>(tribeMember);
            var notifications = tribe.Members
            .Where(x => x.UserId != currentUserId && x.Status == TribeMemberStatus.Joined || x.Status == TribeMemberStatus.Creator)
            .ToList()
            .Select(x => new Notification
            {
                Date = DateTime.UtcNow,
                Type = NotificationTypeEnum.TribeAcceptInvite,
                RecipientId = x.UserId,
                Extra = tribeMemberDTO,
                Handled = true,
                Text = $"[{tribeMember.Tribe.Name}]: {tribeMember.User.UserName} joined the tribe"
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);

            return TribeMemberStatus.Joined;
        }

        public async Task<SharedTribeDTO> GetAsync(string userId, int tribeId)
        {
            var tribe = await _tribeRepository.GetQueryable(true)
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x => x.Id == tribeId)
            .ConfigureAwait(false);

            var result = Mapper.Map<SharedTribeDTO>(tribe, x =>
            {
                x.Items["UserId"] = userId;
                x.Items["Members"] = true;
            });
            return result;
        }

        public async Task<SharedTribeDTO> UpdateAsync(string currentUserId, SharedTribeDTO dto)
        {
            var tribe = await _tribeRepository
            .GetQueryable()
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == dto.Id)
            .ConfigureAwait(false);
            tribe.Name = dto.Name;

            var newMembers = dto.Members
                .Where(member => tribe.Members.All(x => x.UserId != member.UserId))
                .Select(member => new TribeMember
                {
                    UserId = member.UserId,
                    Status = TribeMemberStatus.Invited
                }).ToList();
            newMembers.ForEach(tribe.Members.Add);

            var deletedMembers = tribe.Members
                .Where(member => dto.Members.All(x => x.UserId != member.UserId)).ToList();
            deletedMembers.ForEach(x => tribe.Members.Remove(x));

            _tribeRepository.Save(tribe);
            _tribeMemberRepository.RemoveAll(deletedMembers, true);

            var result = Mapper.Map<SharedTribeDTO>(tribe, x => x.Items["UserId"] = currentUserId);
            await NotifyMembersAboutInvitationToTribeAsync(newMembers, result).ConfigureAwait(false);
            await NotifyMemberDeletedFromTribeAsync(deletedMembers, result);
            result.Members = Mapper.Map<List<SharedTribeMemberDTO>>(tribe.Members);
            return result;
        }

        public async Task<bool> IsTribeCreatorAsync(string userId, int tribeId)
        {
            var result = await _tribeRepository
            .GetQueryable(true)
            .AnyAsync(x => x.Id == tribeId && x.CreatorId == userId)
            .ConfigureAwait(false);
            return result;
        }

        private async Task NotifyMembersAboutInvitationToTribeAsync(IEnumerable<TribeMember> members, SharedTribeDTO result)
        {
            var now = DateTime.UtcNow;
            var notifications = members.Select(x => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.TribeInvite,
                RecipientId = x.UserId,
                Extra = result,
                Text = $"{result.CreatorName} invited you to join a tribe \"{result.Name}\""
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
        }

        private async Task NotifyMemberDeletedFromTribeAsync(IEnumerable<TribeMember> members, SharedTribeDTO result)
        {
            var extra = new object();
            var now = DateTime.UtcNow;
            var notifications = members.Select(x => new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.DeleteFromTribe,
                RecipientId = x.UserId,
                Extra = extra,
                Text = $"You were deleted from a tribe \"{result.Name}\""
            }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
        }
    }
}