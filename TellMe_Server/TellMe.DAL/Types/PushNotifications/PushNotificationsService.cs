using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRepository<ApplicationUser, string> _applicationUserRepository;
        private readonly IRepository<PushNotificationClient, int> _pushTokenRepository;
        private readonly PushSettings _pushSettings;

        public PushNotificationsService(
            IRepository<ApplicationUser, string> applicationUserRepository,
            IRepository<PushNotificationClient, int> pushTokenRepository,
            IOptions<PushSettings> pushSettings)
        {
            _applicationUserRepository = applicationUserRepository;
            _pushTokenRepository = pushTokenRepository;
            this._pushSettings = pushSettings.Value;
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

        public async Task SendStoryRequestPushNotificationAsync(StoryDTO story, string senderName)
        {
            var pushNotificationClients = await _pushTokenRepository
                            .GetQueryable()
                            .AsNoTracking()
                            .Where(a => a.UserId == story.ReceiverId)
                            .ToListAsync()
                            .ConfigureAwait(false);

            var notification = new IosNotification<StoryDTO>
            {
                Data = new IosNotificationAPS
                {
                    Message = $"{senderName} requested a story: {story.Title}"
                },
                Extra = story,
                NotificationType = NotificationTypeEnum.StoryRequest,
            };

            Task.Run(() => SendStoryRequestNotification(notification, pushNotificationClients));
        }

        private void SendStoryRequestNotification(IosNotification<StoryDTO> notification, List<PushNotificationClient> pushNotificationClients)
        {
            var iosTokens = pushNotificationClients.Where(a => a.OsType == OsType.iOS).ToList();
            SendIosPushNotification(iosTokens, notification);
        }

        private ApnsConfiguration GetApnsConfig()
        {
            var environment = _pushSettings.IsProductionMode
                               ? ApnsConfiguration.ApnsServerEnvironment.Production
                               : ApnsConfiguration.ApnsServerEnvironment.Sandbox;
            var config = new ApnsConfiguration(environment, _pushSettings.Certificate, _pushSettings.Password);
            return config;
        }

        public void SendIosPushNotification<T>(List<PushNotificationClient> pushNotificationClient, IosNotification<T> notification)
        {
            var payload = JObject.FromObject(notification, new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            });

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

            foreach (var token in pushNotificationClient)
            {
                broker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = token.Token,
                    Payload = payload
                });
            }
            broker.Stop();
        }
    }
}