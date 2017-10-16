using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class NotificationCenterBusinessLogic
    {
        private readonly INotificationsCenterView _view;
        private readonly RemoteNotificationsDataService _remoteNotificationsDataService;
        private readonly RemoteStorytellersDataService _remoteStorytellersDataService;
        private readonly LocalNotificationsDataService _localNotificationsDataService;
        private readonly List<NotificationDTO> notifications = new List<NotificationDTO>();

        public NotificationCenterBusinessLogic(RemoteStorytellersDataService remoteStorytellersDataService, RemoteNotificationsDataService remoteNotificationsDataService, INotificationsCenterView view)
        {
            _remoteStorytellersDataService = remoteStorytellersDataService;
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _localNotificationsDataService = new LocalNotificationsDataService();
            _view = view;
        }

        public async Task LoadNotificationsAsync(bool forceRefresh = false)
        {
            var localEntities = await _localNotificationsDataService.GetAllAsync().ConfigureAwait(false);
            if (localEntities.Expired || forceRefresh)
            {
                if (forceRefresh)
                {
                    notifications.Clear();
                }

                var result = await _remoteNotificationsDataService.GetNotificationsAsync(notifications.Count).ConfigureAwait(false);
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

        public async Task RejectFriendshipRequestAsync(NotificationDTO notification, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.RejectFriendshipRequestAsync(dto.Id, notification.Id).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                notification.Handled = true;
                await _localNotificationsDataService.SaveAsync(notification).ConfigureAwait(false);
                this._view.ReloadNotification(notification);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }

        public async Task AcceptFriendshipRequestAsync(NotificationDTO notification, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.AcceptFriendshipRequestAsync(dto.Id, notification.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                notification.Handled = true;
                await _localNotificationsDataService.SaveAsync(notification).ConfigureAwait(false);
                this._view.ReloadNotification(notification);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }
    }
}
