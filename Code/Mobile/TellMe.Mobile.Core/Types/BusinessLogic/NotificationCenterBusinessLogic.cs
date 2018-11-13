using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class NotificationCenterBusinessLogic : INotificationCenterBusinessLogic
    {
        private readonly IRemoteNotificationsDataService _remoteNotificationsDataService;

        private readonly List<NotificationDTO> _notifications = new List<NotificationDTO>();

        private readonly INotificationHandler _notificationHandler;

        public INotificationsCenterView View { get; set; }

        public NotificationCenterBusinessLogic(INotificationHandler notificationHandler,
            IRemoteNotificationsDataService remoteNotificationsDataService)
        {
            _remoteNotificationsDataService = remoteNotificationsDataService;
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
                NotificationProcessed(notification.Id, result.Value);
        }

        private void NotificationProcessed(int notificationId, bool success)
        {
            var notification = _notifications.FirstOrDefault(x => x.Id == notificationId);
            if (notification != null)
            {
                notification.Handled = success;
                View.ReloadNotification(notification);
            }
        }
    }
}