using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TellMe.DAL.Contracts.DTO;
using AutoMapper.QueryableExtensions;
using System;
using TellMe.DAL.Contracts.PushNotification;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<Friendship, int> _friendshipRepository;
        private readonly IRepository<RefreshToken, int> _refreshTokenRepository;

        public UserService(
            IRepository<ApplicationUser, string> serviceRepository,
            IRepository<RefreshToken, int> refreshTokenRepository,
            IRepository<Friendship, int> friendshipRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<Notification, int> notificationRepository)
        {
            _userRepository = serviceRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _friendshipRepository = friendshipRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
        }
        public async Task<ApplicationUser> GetAsync(string id)
        {
            var result = await _userRepository.GetQueryable().SingleAsync(a => a.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<IReadOnlyCollection<StorytellerDTO>> SearchAsync(string currentUserId, string fragment)
        {
            var uppercaseFragment = fragment.ToUpper();
            var words = uppercaseFragment.Split(' ');
            var users = _userRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.Id != currentUserId &&
            (x.NormalizedUserName.StartsWith(uppercaseFragment)
            || x.NormalizedEmail == uppercaseFragment
            || words.All(y => x.FullName.ToUpper().Contains(y))));

            var friends = _friendshipRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == currentUserId);

            var result = await (from user in users
                                join friend in friends on user.Id equals friend.FriendId into gj
                                from x in gj.DefaultIfEmpty()
                                select new StorytellerDTO
                                {
                                    Id = user.Id,
                                    UserName = user.UserName,
                                    FullName = user.FullName,
                                    PictureUrl = user.PictureUrl,
                                    FriendshipStatus = x != null ? x.Status : FriendshipStatus.None
                                }).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<IReadOnlyCollection<StorytellerDTO>> GetAllFriendsAsync(string currentUserId)
        {
            var result = await _friendshipRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == currentUserId)
            .Select(x => new StorytellerDTO
            {
                Id = x.FriendId,
                UserName = x.Friend.UserName,
                FullName = x.Friend.FullName,
                PictureUrl = x.Friend.PictureUrl,
                FriendshipStatus = x != null ? x.Status : FriendshipStatus.None
            }).ToListAsync().ConfigureAwait(false);
            return result;
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
            return _refreshTokenRepository.GetQueryable().FirstOrDefaultAsync(x => x.ClientId == clientId && x.Token == token);
        }

        public async Task<FriendshipStatus> AddToFriendsAsync(string currentUserId, string userId)
        {
            var friendships = await _friendshipRepository.GetQueryable()
            .Where(x => (x.UserId == currentUserId && x.FriendId == userId) || (x.UserId == userId && x.FriendId == currentUserId))
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

            var user = await _userRepository.GetQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentUserId).ConfigureAwait(false);
            var friend = new StorytellerDTO
            {
                Id = myFriendship.UserId,
                FriendshipStatus = myFriendship.Status,
                UserName = user.UserName,
                FullName = user.FullName,
                PictureUrl = user.PictureUrl
            };
            if (friendships.Count == 0)
            {
                var notification = new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.FriendshipRequest,
                    RecipientId = myFriendship.FriendId,
                    Extra = friend,
                    Text = $"{friend.UserName} sent you a friendship request"
                };

                await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
                await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);
            }
            else
            {
                var notification = new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.FriendshipAccepted,
                    RecipientId = myFriendship.FriendId,
                    Extra = friend,
                    Text = $"{friend.UserName} accepted your friendship request"
                };

                await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
                await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);
            }

            return myFriendship.Status;
        }

        public async Task<FriendshipStatus> RejectFriendshipRequestAsync(string currentUserId, string userId)
        {
            var friendships = await _friendshipRepository.GetQueryable()
            .Where(x => (x.UserId == currentUserId && x.FriendId == userId) || (x.UserId == userId && x.FriendId == currentUserId))
            .ToListAsync().ConfigureAwait(false);

            DateTime now = DateTime.UtcNow;
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

            await _notificationRepository.SaveAsync(notification, true).ConfigureAwait(false);
            await _pushNotificationsService.SendPushNotificationAsync(notification).ConfigureAwait(false);

            return FriendshipStatus.Rejected;
        }
    }
}