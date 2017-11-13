using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class NotificationCenterBusinessLogic
    {
        readonly INotificationsCenterView _view;
        readonly RemoteNotificationsDataService _remoteNotificationsDataService;

        readonly LocalNotificationsDataService _localNotificationsDataService;

        readonly List<NotificationDTO> notifications = new List<NotificationDTO>();

        readonly INotificationHandler _notificationHandler;
        public NotificationCenterBusinessLogic(INotificationHandler notificationHandler,
                                               RemoteNotificationsDataService remoteNotificationsDataService,
                                               INotificationsCenterView view)
        {
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _notificationHandler = notificationHandler;
            _localNotificationsDataService = new LocalNotificationsDataService();
            _view = view;
        }

        public async Task LoadNotificationsAsync(bool forceRefresh = false)
        {
            //await _localNotificationsDataService.DeleteAllAsync();
            var localEntities = await _localNotificationsDataService.GetAllAsync().ConfigureAwait(false);
            if (localEntities.Expired || forceRefresh)
            {
                if (forceRefresh)
                {
                    notifications.Clear();
                }

                var result = await _remoteNotificationsDataService.GetNotificationsAsync(notifications.LastOrDefault()?.Date).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localNotificationsDataService.SaveAllAsync(result.Data).ConfigureAwait(false);
                    notifications.AddRange(result.Data);
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }
            else
            {
                notifications.Clear();
                notifications.AddRange(localEntities.Data);
            }

            this._view.DisplayNotifications(notifications);
        }

        public void HandleNotification(NotificationDTO notification)
        {
            _notificationHandler.ProcessNotification(notification, _view, NotificationProcessed);
        }

        async void NotificationProcessed(int notificationId, bool success)
        {
            var notification = notifications.FirstOrDefault(x => x.Id == notificationId);
            notification.Handled = true;
            await _localNotificationsDataService.SaveAsync(notification).ConfigureAwait(false);
            _view.ReloadNotification(notification);
        }
    }
}
