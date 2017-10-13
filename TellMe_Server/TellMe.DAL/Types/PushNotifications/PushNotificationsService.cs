using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.PushNotification;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Types.PushNotifications;
using TellMe.DAL.Types.Repositories;

namespace TellMe.DAL.Types.Services
{
    public class PushNotificationsService : IPushNotificationsService
    {
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<PushNotificationClient, int> _pushTokenRepository;
        private readonly PushSettings _pushSettings;

        private readonly IHostingEnvironment _environment;

        public PushNotificationsService(
            IRepository<ApplicationUser, string> UserRepository,
            IRepository<PushNotificationClient, int> pushTokenRepository,
            IOptions<PushSettings> pushSettings,
            IHostingEnvironment environment)
        {
            _userRepository = UserRepository;
            _pushTokenRepository = pushTokenRepository;
            _pushSettings = pushSettings.Value;
            _environment = environment;
        }

        public async Task RegisterPushTokenAsync(string token, string oldToken, OsType osType, string userId, string appVersion)
        {
            PushNotificationClient deviceToken = null;
            if (oldToken != null)
            {
                deviceToken = await _pushTokenRepository
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.UserId == userId && a.OsType == osType && a.Token.ToUpper() == oldToken.ToUpper())
                .ConfigureAwait(false);
            }

            if (deviceToken == null)
            {
                _pushTokenRepository.Save(new PushNotificationClient
                {
                    OsType = osType,
                    Token = token,
                    AppVersion = appVersion,
                    UserId = userId
                });
            }
            else
            {
                deviceToken.Token = token;
                deviceToken.AppVersion = appVersion;
            }

            _pushTokenRepository.PreCommitSave();
        }

        public async Task SendStoryRequestPushNotificationAsync(IReadOnlyCollection<StoryDTO> storyDTOs, string requestSenderId)
        {
            var receiverIds = storyDTOs.Select(x => x.SenderId).ToArray();
            var pushNotificationClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => receiverIds.Contains(a.UserId) && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var user = await _userRepository
                        .GetQueryable()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == requestSenderId)
                        .ConfigureAwait(false);

            var notifications = storyDTOs.Select(storyDTO =>
            {
                var notification = new IosNotification<StoryDTO>
                {
                    Data = new IosNotificationAPS
                    {
                        Message = $"{user.UserName} requested a story: {storyDTO.Title}"
                    },
                    Extra = storyDTO,
                    NotificationType = NotificationTypeEnum.StoryRequest,
                };

                var pushClients = pushNotificationClients.Where(x => x.UserId == storyDTO.SenderId).ToArray();
                return new Tuple<IosNotification<StoryDTO>, IReadOnlyCollection<PushNotificationClient>>(notification, pushClients);
            }).ToArray();

            Task.Run(() => SendIosPushNotification<StoryDTO>(notifications));
        }

        public async Task SendStoryPushNotificationAsync(IReadOnlyCollection<StoryDTO> storyDTOs, string senderId)
        {
            var receiverIds = storyDTOs.Select(x => x.ReceiverId).ToArray();
            var pushNotificationClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => receiverIds.Contains(a.UserId) && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == senderId)
            .ConfigureAwait(false);

            var notifications = storyDTOs.Select(storyDTO =>
            {
                var notification = new IosNotification<StoryDTO>
                {
                    Data = new IosNotificationAPS
                    {
                        Message = $"{user.UserName} sent a story: {storyDTO.Title}"
                    },
                    Extra = storyDTO,
                    NotificationType = NotificationTypeEnum.Story,
                };
                var pushClients = pushNotificationClients.Where(x => x.UserId == storyDTO.ReceiverId).ToArray();
                return new Tuple<IosNotification<StoryDTO>, IReadOnlyCollection<PushNotificationClient>>(notification, pushClients);
            }).ToArray();

            Task.Run(() => SendIosPushNotification<StoryDTO>(notifications));
        }

        public async Task SendFriendshipRequestPushNotificationAsync(StorytellerDTO friendshipInitiator, string friendId)
        {
            var pushClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => a.UserId == friendId && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var notification = new IosNotification<StorytellerDTO>
            {
                Data = new IosNotificationAPS
                {
                    Message = $"{friendshipInitiator.UserName} sent you a friendship request"
                },
                Extra = friendshipInitiator,
                NotificationType = NotificationTypeEnum.FriendshipRequest,
            };

            Task.Run(() => SendIosPushNotification<StorytellerDTO>(notification, pushClients));
        }

        public async Task SendFriendshipAcceptedPushNotificationAsync(StorytellerDTO friendshipAcceptor, string friendId)
        {
            var pushClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => a.UserId == friendId && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var notification = new IosNotification<StorytellerDTO>
            {
                Data = new IosNotificationAPS
                {
                    Message = $"{friendshipAcceptor.UserName} accepted your friendship request"
                },
                Extra = friendshipAcceptor,
                NotificationType = NotificationTypeEnum.FriendshipAccepted,
            };

            Task.Run(() => SendIosPushNotification<StorytellerDTO>(notification, pushClients));
        }

        public async Task SendFriendshipRejectedPushNotificationAsync(StorytellerDTO friendshipRejector, string friendId)
        {
            var pushClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => a.UserId == friendId && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var notification = new IosNotification<StorytellerDTO>
            {
                Data = new IosNotificationAPS
                {
                    Message = $"{friendshipRejector.UserName} rejected your friendship request"
                },
                Extra = friendshipRejector,
                NotificationType = NotificationTypeEnum.FriendshipRejected,
            };

            Task.Run(() => SendIosPushNotification<StorytellerDTO>(notification, pushClients));
        }

        public void SendIosPushNotification<T>(IosNotification<T> notification, IReadOnlyCollection<PushNotificationClient> pushClients)
        {
            SendIosPushNotification<T>(new Tuple<IosNotification<T>, IReadOnlyCollection<PushNotificationClient>>(notification, pushClients));
        }

        public void SendIosPushNotification<T>(params Tuple<IosNotification<T>, IReadOnlyCollection<PushNotificationClient>>[] notifications)
        {
            var config = GetApnsConfig();
            var broker = new ApnsServiceBroker(config);
            broker.OnNotificationFailed += (n, exception) =>
            {
                exception.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        if (statusCode == ApnsNotificationErrorStatusCode.InvalidToken)
                        {
                            // TODO: Remove invalid token from DB
                        }

                        //Log.Warn(exception, $"Can not send notification by token: {n.DeviceToken}, ID={apnsNotification.Identifier}, Code={statusCode}");
                    }
                    else
                    {
                        //Log.Error(ex.InnerException, $"Can not send notification by token: {n.DeviceToken}, Reason: unknown");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            broker.Start();

            foreach (var notification in notifications)
            {
                var payload = JObject.FromObject(notification.Item1, new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                foreach (var token in notification.Item2)
                {
                    broker.QueueNotification(new ApnsNotification
                    {
                        DeviceToken = token.Token,
                        Payload = payload
                    });
                }
            }

            broker.Stop();
        }

        private ApnsConfiguration GetApnsConfig()
        {
            var environment = _pushSettings.IsProductionMode
                               ? ApnsConfiguration.ApnsServerEnvironment.Production
                               : ApnsConfiguration.ApnsServerEnvironment.Sandbox;
            var config = new ApnsConfiguration(environment, Path.Combine(_environment.ContentRootPath, _pushSettings.Certificate), _pushSettings.Password);
            return config;
        }
    }
}