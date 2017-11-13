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

        public async Task SendPushNotificationAsync(params Notification[] notifications)
        {
            var receiverIds = notifications.Select(x => x.RecipientId).ToArray();
            var pushNotificationClients = await _pushTokenRepository
                                        .GetQueryable()
                                        .AsNoTracking()
                                        .Where(a => receiverIds.Contains(a.UserId) && a.OsType == OsType.iOS)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            var pushNotifications = notifications.Select(n =>
            {
                var notification = new IosNotification<object>
                {
                    Data = new IosNotificationAPS
                    {
                        Message = n.Text
                    },
                    Extra = n.Extra,
                    NotificationId = n.Id,
                    NotificationType = n.Type,
                };

                var pushClients = pushNotificationClients.Where(x => x.UserId == n.RecipientId).ToArray();
                return new Tuple<IosNotification<object>, IReadOnlyCollection<PushNotificationClient>>(notification, pushClients);
            }).ToArray();

            Task.Run(() => SendIosPushNotification<object>(pushNotifications));
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