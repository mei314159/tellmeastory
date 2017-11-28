using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class NotificationCenterBusinessLogic : INotificationCenterBusinessLogic
    {
        private readonly IRemoteNotificationsDataService _remoteNotificationsDataService;

        private readonly ILocalNotificationsDataService _localNotificationsDataService;

        private readonly List<NotificationDTO> _notifications = new List<NotificationDTO>();

        private readonly INotificationHandler _notificationHandler;

        public INotificationsCenterView View { get; set; }

        public NotificationCenterBusinessLogic(INotificationHandler notificationHandler,
            IRemoteNotificationsDataService remoteNotificationsDataService,
            ILocalNotificationsDataService localNotificationsDataService)
        {
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _localNotificationsDataService = localNotificationsDataService;
            _notificationHandler = notificationHandler;
        }

        public async Task LoadNotificationsAsync(bool forceRefresh = false)
        {
            //await _localNotificationsDataService.DeleteAllAsync();
            //var localEntities = await _localNotificationsDataService.GetAllAsync().ConfigureAwait(false);

            if (forceRefresh)
            {
                _notifications.Clear();
            }

            var result = await _remoteNotificationsDataService
                .GetNotificationsAsync(_notifications.LastOrDefault()?.Date).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localNotificationsDataService.SaveAllAsync(result.Data).ConfigureAwait(false);
                _notifications.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayNotifications(_notifications);
        }

        public async void HandleNotification(NotificationDTO notification)
        {
            var result = await _notificationHandler.ProcessNotificationAsync(notification, View).ConfigureAwait(false);
            if (result.HasValue)
                await NotificationProcessed(notification.Id, result.Value).ConfigureAwait(false);
        }

        private async Task NotificationProcessed(int notificationId, bool success)
        {
            var notification = _notifications.FirstOrDefault(x => x.Id == notificationId);
            notification.Handled = success;
            await _localNotificationsDataService.SaveAsync(notification).ConfigureAwait(false);
            View.ReloadNotification(notification);
        }
    }
}