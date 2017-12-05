using System;
using System.Linq;
using Microsoft.Extensions.Options;
using PushSharp.Apple;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Types.PushNotifications
{
    public class PushFeedbackService
    {
        private readonly IRepository<PushNotificationClient, int> _pushTokenRepository;
        private readonly PushSettings _pushSettings;

        public PushFeedbackService(IRepository<PushNotificationClient, int> pushTokenRepository,
            IOptions<PushSettings> pushSettings)
        {
            _pushTokenRepository = pushTokenRepository;
            this._pushSettings = pushSettings.Value;
        }

        public void CheckExpiredTokens()
        {
            var config = GetApnsConfig();
            var service = new FeedbackService(config);
            service.FeedbackReceived += ServiceOnFeedbackReceived;
            service.Check();
        }

        private void ServiceOnFeedbackReceived(string deviceToken, DateTime timestamp)
        {
            var expiredTokens = _pushTokenRepository.GetQueryable().Where(a => a.Token == deviceToken).ToList();
            foreach (var token in expiredTokens)
            {
                _pushTokenRepository.Remove(token);
            }
            _pushTokenRepository.Commit();
        }

        private ApnsConfiguration GetApnsConfig()
        {
            var environment = _pushSettings.IsProductionMode
                               ? ApnsConfiguration.ApnsServerEnvironment.Production
                               : ApnsConfiguration.ApnsServerEnvironment.Sandbox;
            var config = new ApnsConfiguration(environment, _pushSettings.Certificate, _pushSettings.Password);
            return config;
        }
    }
}