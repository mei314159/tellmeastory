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
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Types.PushNotifications;
using TellMe.DAL.Types.Repositories;

namespace TellMe.DAL.Types.Services
{
    public class PushFeedbackService
    {
        private readonly IRepository<ApplicationUser, string> _applicationUserRepository;
        private readonly IRepository<PushNotificationClient, int> _pushTokenRepository;
        private readonly PushSettings _pushSettings;

        public PushFeedbackService(
            IRepository<ApplicationUser, string> applicationUserRepository,
            IRepository<PushNotificationClient, int> pushTokenRepository,
            IOptions<PushSettings> pushSettings)
        {
            _applicationUserRepository = applicationUserRepository;
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