using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.Core.Types.BusinessLogic
{
    public class NotificationHandlerBusinessLogic
    {
        readonly RemoteNotificationsDataService _remoteNotificationsDataService;
        readonly RemoteStorytellersDataService _remoteStorytellersDataService;
        readonly RemoteStoriesDataService _remoteStoriesDataService;
        readonly RemoteTribesDataService _remoteTribesDataService;
        readonly LocalStorytellersDataService _localStorytellersDataService;
        readonly IRouter _router;

        public NotificationHandlerBusinessLogic(IRouter router,
                                               RemoteStoriesDataService remoteStoriesDataService,
                                               RemoteStorytellersDataService remoteStorytellersDataService,
                                               RemoteNotificationsDataService remoteNotificationsDataService,
                                               RemoteTribesDataService remoteTribesDataService)
        {
            _router = router;
            _remoteStorytellersDataService = remoteStorytellersDataService;
            _remoteStoriesDataService = remoteStoriesDataService;
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _remoteTribesDataService = remoteTribesDataService;
            _localStorytellersDataService = new LocalStorytellersDataService();
        }

        public async Task RejectFriendshipRequestAsync(int notificationId, StorytellerDTO dto, Action<int, bool> complete)
        {
            var result = await _remoteStorytellersDataService.RejectFriendshipRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            complete?.Invoke(notificationId, result.IsSuccess);
        }

        public async Task AcceptFriendshipRequestAsync(int notificationId, StorytellerDTO dto, Action<int, bool> complete)
        {
            var result = await _remoteStorytellersDataService.AcceptFriendshipRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.FriendshipStatus = result.Data;
                await _localStorytellersDataService.SaveAsync(dto).ConfigureAwait(false);
            }

            complete?.Invoke(notificationId, result.IsSuccess);
        }

        public async Task RejectTribeInvitationAsync(int notificationId, TribeDTO dto, Action<int, bool> complete)
        {
            var result = await _remoteTribesDataService.RejectTribeInvitationAsync(dto.Id, notificationId).ConfigureAwait(false);

            complete?.Invoke(notificationId, result.IsSuccess);
        }

        public async Task AcceptTribeInvitationAsync(int notificationId, TribeDTO dto, Action<int, bool> complete)
        {
            var result = await _remoteTribesDataService.AcceptTribeInvitationAsync(dto.Id, notificationId).ConfigureAwait(false);

            complete?.Invoke(notificationId, result.IsSuccess);
        }

        public async Task ViewStory(int notificationId, StoryDTO story, IView view, Action<int, bool> complete)
        {
            await _remoteNotificationsDataService.HandleNotificationAsync(notificationId).ConfigureAwait(false);
			_router.NavigateViewStory(view, story);
            complete?.Invoke(notificationId, true);
        }

        public async Task RejectStoryRequestRequestAsync(int notificationId, StoryRequestDTO dto, Action<int, bool> complete)
        {
            var result = await _remoteStoriesDataService.RejectStoryRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            complete?.Invoke(notificationId, result.IsSuccess);
        }

        public async Task AcceptStoryRequestRequest(int notificationId, StoryRequestDTO dto, IView view, Action<int, bool> complete)
        {
            await _remoteNotificationsDataService.HandleNotificationAsync(notificationId).ConfigureAwait(false);
            _router.NavigateRecordStory(view, dto);
            complete?.Invoke(notificationId, true);
        }
    }
}
