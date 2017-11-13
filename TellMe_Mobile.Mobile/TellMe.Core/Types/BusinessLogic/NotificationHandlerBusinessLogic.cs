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

        public async Task<bool> RejectFriendshipRequestAsync(int notificationId, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.RejectFriendshipRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptFriendshipRequestAsync(int notificationId, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.AcceptFriendshipRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.FriendshipStatus = result.Data;
                await _localStorytellersDataService.SaveAsync(dto).ConfigureAwait(false);
            }

            return result.IsSuccess;
        }

        public async Task<bool> RejectTribeInvitationAsync(int notificationId, TribeDTO dto)
        {
            var result = await _remoteTribesDataService.RejectTribeInvitationAsync(dto.Id, notificationId).ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptTribeInvitationAsync(int notificationId, TribeDTO dto)
        {
            var result = await _remoteTribesDataService.AcceptTribeInvitationAsync(dto.Id, notificationId).ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> ViewStory(int notificationId, StoryDTO story, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId).ConfigureAwait(false);
			_router.NavigateViewStory(view, story);
            return result.IsSuccess;
        }

        public async Task<bool> RejectStoryRequestRequestAsync(int notificationId, StoryRequestDTO dto)
        {
            var result = await _remoteStoriesDataService.RejectStoryRequestAsync(dto.Id, notificationId).ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptStoryRequestRequest(int notificationId, StoryRequestDTO dto, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId).ConfigureAwait(false);
            _router.NavigateRecordStory(view, dto);
            return result.IsSuccess;
        }

        public async Task<bool> HandleNotification(int notificationId)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId).ConfigureAwait(false);
            return result.IsSuccess;
        }
    }
}
