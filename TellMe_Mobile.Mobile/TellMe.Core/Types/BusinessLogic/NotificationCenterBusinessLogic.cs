using System;
using System.Collections.Generic;
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
        readonly RemoteStorytellersDataService _remoteStorytellersDataService;
        readonly RemoteStoriesDataService _remoteStoriesDataService;
        readonly LocalNotificationsDataService _localNotificationsDataService;
        readonly LocalStoriesDataService _localStoriesDataService;
        readonly LocalStorytellersDataService _localStorytellersDataService;
        readonly List<NotificationDTO> notifications = new List<NotificationDTO>();
        readonly IRouter _router;

        public NotificationCenterBusinessLogic(IRouter router, RemoteStoriesDataService remoteStoriesDataService, RemoteStorytellersDataService remoteStorytellersDataService, RemoteNotificationsDataService remoteNotificationsDataService, INotificationsCenterView view)
        {
            _router = router;
            _remoteStorytellersDataService = remoteStorytellersDataService;
            _remoteStoriesDataService = remoteStoriesDataService;
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _localNotificationsDataService = new LocalNotificationsDataService();
            _localStoriesDataService = new LocalStoriesDataService();
            _localStorytellersDataService = new LocalStorytellersDataService();
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

                dto.FriendshipStatus = result.Data;
                await _localStorytellersDataService.SaveAsync(dto).ConfigureAwait(false);
                this._view.ReloadNotification(notification);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }

        public async Task RejectStoryRequestRequestAsync(NotificationDTO notification, StoryDTO dto)
        {
            var result = await _remoteStoriesDataService.RejectStoryRequestAsync(dto.Id, notification.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                notification.Handled = true;
                await _localNotificationsDataService.SaveAsync(notification).ConfigureAwait(false);

                dto.Status = result.Data;
                await _localStoriesDataService.SaveAsync(dto).ConfigureAwait(false);

                this._view.ReloadNotification(notification);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }

        public void AcceptStoryRequestRequest(NotificationDTO notification, StoryDTO dto)
        {
            _router.NavigateRecordStory(_view, dto, notification);
        }
    }
}
