using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TellMe.Web.DAL.Contracts;
using TellMe.Web.DAL.Contracts.DTO;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;
using TellMe.Web.DAL.Types.Settings;

namespace TellMe.Web.DAL.Types.Services
{
    public class UserService : IUserService
    {
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<Friendship, int> _friendshipRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        private readonly IRepository<RefreshToken, int> _refreshTokenRepository;
        private readonly IMailSender _mailSender;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly AppSettings _appSettings;

        public UserService(
            IRepository<ApplicationUser, string> userRepository,
            IRepository<RefreshToken, int> refreshTokenRepository,
            IRepository<Friendship, int> friendshipRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<TribeMember, int> tribeMemberRepository,
            IMailSender mailSender,
            IOptions<AppSettings> emailingSettings,
            IStringLocalizer stringLocalizer)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _friendshipRepository = friendshipRepository;
            _pushNotificationsService = pushNotificationsService;
            _tribeMemberRepository = tribeMemberRepository;
            _mailSender = mailSender;
            _stringLocalizer = stringLocalizer;
            _appSettings = emailingSettings.Value;
        }

        public async Task<ApplicationUser> GetAsync(string id)
        {
            var result = await _userRepository.GetQueryable().SingleAsync(a => a.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<IReadOnlyCollection<ContactDTO>> SearchContactsAsync(string currentUserId, string fragment,
            ContactsMode mode, int skip)
        {
            string uppercaseFragment = null;
            var userQuery = _userRepository
                .GetQueryable(true)
                .Where(x => x.Id != currentUserId);

            if (!string.IsNullOrWhiteSpace(fragment))
            {
                uppercaseFragment = fragment.ToUpper();
                var words = uppercaseFragment.Split(' ');

                userQuery = userQuery
                    .Where(x => x.NormalizedUserName.StartsWith(uppercaseFragment)
                                || x.NormalizedEmail == uppercaseFragment
                                || words.All(y => x.FullName.ToUpper().Contains(y)));
            }

            var friends = _friendshipRepository
                .GetQueryable(true)
                .Where(x => x.UserId == currentUserId);

            var users =
            (from user in userQuery
                join friend in friends on user.Id equals friend.FriendId into gj
                from x in gj.DefaultIfEmpty()
                select new ContactDTO
                {
                    Type = ContactType.User,
                    Name = user.UserName,
                    UserId = user.Id,
                    User = new StorytellerDTO
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        PictureUrl = user.PictureUrl,
                        FriendshipStatus = x != null ? x.Status : FriendshipStatus.None
                    }
                });

            if (string.IsNullOrWhiteSpace(fragment) || mode != ContactsMode.Normal)
            {
                users = users.Where(x => x.User.FriendshipStatus == FriendshipStatus.Accepted);
            }

            IQueryable<ContactDTO> contacts;
            if (mode == ContactsMode.FriendsOnly)
            {
                contacts = users;
            }
            else
            {
                var tribeQuery = _tribeMemberRepository
                    .GetQueryable(true)
                    .Include(x => x.Tribe)
                    .Where(x => x.UserId == currentUserId && x.Status != TribeMemberStatus.Rejected);
                if (!string.IsNullOrWhiteSpace(fragment))
                {
                    tribeQuery = tribeQuery.Where(x => x.Tribe.Name.ToUpper().StartsWith(uppercaseFragment));
                }
                var tribes = tribeQuery.Select(x => new ContactDTO
                {
                    Type = ContactType.Tribe,
                    Name = x.Tribe.Name,
                    TribeId = x.TribeId,
                    Tribe = new TribeDTO
                    {
                        Id = x.TribeId,
                        Name = x.Tribe.Name,
                        CreateDateUtc = x.Tribe.CreateDateUtc,
                        MembershipStatus = x.Status,
                        CreatorId = x.Tribe.CreatorId,
                        CreatorName = x.Tribe.Creator.UserName,
                        CreatorPictureUrl = x.Tribe.Creator.PictureUrl
                    }
                });

                contacts = users.Concat(tribes);
            }

            var result = await contacts.OrderBy(x => x.Type).ThenBy(x => x.Name).Skip(skip).ToListAsync()
                .ConfigureAwait(false);
            return result;
        }

        public async Task SendRegistrationConfirmationEmailAsync(string userId, string email, string confirmationToken)
        {
            const string displayName = "TellMeAStoryApp";
            const string mailSubject = "Registration confirmation";

            var mailBody = string.Format(
                _stringLocalizer["RegistrationConfirmation_MailTemplate"],
                $"{_appSettings.Host}/api/account/confirm/{userId}/{confirmationToken}",
                _appSettings.Host);

            await _mailSender
                .SendAsync(_appSettings.SupportEmail, displayName, mailSubject, mailBody, new[] {email}, true)
                .ConfigureAwait(false);
        }

        public async Task SendRequestToJoinAsync(string email, string senderUserId)
        {
            var user = await _userRepository.GetQueryable(true).FirstOrDefaultAsync(x => x.Id == senderUserId)
                .ConfigureAwait(false);

            const string displayName = "TellMeAStoryApp";
            const string mailSubject = "Registration confirmation";

            var mailBody = string.Format(
                _stringLocalizer["RequestToJoin_MailTemplate"],
                user.FullName,
                $"{_appSettings.Host}/account/signup",
                _appSettings.Host);

            await _mailSender
                .SendAsync(_appSettings.SupportEmail, displayName, mailSubject, mailBody, new[] {email}, true)
                .ConfigureAwait(false);
        }

        public async Task<bool> AddTokenAsync(RefreshToken token)
        {
            await _refreshTokenRepository.SaveAsync(token, true).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> ExpireTokenAsync(RefreshToken token)
        {
            await _refreshTokenRepository.SaveAsync(token).ConfigureAwait(false);
            return true;
        }

        public Task<RefreshToken> GetTokenAsync(string token, string clientId)
        {
            return _refreshTokenRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.Token == token);
        }

        public async Task<FriendshipStatus> AddToFriendsAsync(string currentUserId, string userId)
        {
            var friendships = await _friendshipRepository.GetQueryable()
                .Where(x => (x.UserId == currentUserId && x.FriendId == userId) ||
                            (x.UserId == userId && x.FriendId == currentUserId))
                .ToListAsync().ConfigureAwait(false);

            DateTime now = DateTime.UtcNow;
            Friendship myFriendship = null;
            if (friendships.Count == 0)
            {
                myFriendship = new Friendship
                {
                    UserId = currentUserId,
                    FriendId = userId,
                    Status = FriendshipStatus.Requested,
                    UpdateDate = now
                };
                _friendshipRepository.Save(myFriendship);
                _friendshipRepository.Save(new Friendship
                {
                    UserId = userId,
                    FriendId = currentUserId,
                    Status = FriendshipStatus.WaitingForResponse,
                    UpdateDate = now
                });
            }
            else
            {
                foreach (var friendship in friendships)
                {
                    // Another user initiated friendship
                    if (friendship.UserId == currentUserId)
                    {
                        if (friendship.Status == FriendshipStatus.WaitingForResponse)
                        {
                            friendship.Status = FriendshipStatus.Accepted;
                            friendship.UpdateDate = now;
                        }
                        myFriendship = friendship;
                    }

                    if (friendship.UserId == userId && friendship.Status == FriendshipStatus.Requested)
                    {
                        friendship.Status = FriendshipStatus.Accepted;
                        friendship.UpdateDate = now;
                    }
                }
            }

            _friendshipRepository.PreCommitSave();

            var user = await _userRepository.GetQueryable().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
            var friend = new StorytellerDTO
            {
                Id = myFriendship.UserId,
                FriendshipStatus = myFriendship.Status,
                UserName = user.UserName,
                FullName = user.FullName,
                PictureUrl = user.PictureUrl
            };
            Notification notification;
            if (friendships.Count == 0)
            {
                notification = new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.FriendshipRequest,
                    RecipientId = myFriendship.FriendId,
                    Extra = friend,
                    Text = $"{friend.UserName} sent you a friendship request"
                };
            }
            else
            {
                notification = new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.FriendshipAccepted,
                    RecipientId = myFriendship.FriendId,
                    Extra = friend,
                    Text = $"{friend.UserName} accepted your friendship request"
                };
            }
            
            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);

            return myFriendship.Status;
        }

        public async Task<FriendshipStatus> RejectFriendshipRequestAsync(string currentUserId, string userId)
        {
            var friendships = await _friendshipRepository.GetQueryable()
                .Where(x => (x.UserId == currentUserId && x.FriendId == userId) ||
                            (x.UserId == userId && x.FriendId == currentUserId))
                .ToListAsync().ConfigureAwait(false);

            var now = DateTime.UtcNow;
            Friendship myFriendship = null;
            if (friendships.Count > 0)
            {
                foreach (var friendship in friendships)
                {
                    _friendshipRepository.Remove(friendship);
                }
            }

            _friendshipRepository.PreCommitSave();

            var friend = new StorytellerDTO
            {
                Id = myFriendship.UserId,
                UserName = myFriendship.User.UserName,
                FullName = myFriendship.User.FullName,
                PictureUrl = myFriendship.User.PictureUrl,
                FriendshipStatus = myFriendship.Status
            };

            var notification = new Notification
            {
                Date = now,
                Type = NotificationTypeEnum.FriendshipRejected,
                RecipientId = myFriendship.FriendId,
                Extra = friend,
                Text = $"{friend.UserName} rejected your friendship request"
            };

            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);

            return FriendshipStatus.Rejected;
        }

        public async Task<StorytellerDTO> GetStorytellerAsync(string currentUserId, string userId)
        {
            var user = await _userRepository
                .GetQueryable(true)
                .FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);

            var friendship = await _friendshipRepository
                .GetQueryable(true)
                .FirstOrDefaultAsync(x => x.UserId == currentUserId).ConfigureAwait(false);

            var result = new StorytellerDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                PictureUrl = user.PictureUrl,
                FriendshipStatus = friendship?.Status ?? FriendshipStatus.None
            };

            return result;
        }
    }
}