using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class NotificationHandlerBusinessLogic : INotificationHandlerBusinessLogic
    {
        private readonly IRemoteNotificationsDataService _remoteNotificationsDataService;
        private readonly IRemoteStorytellersDataService _remoteStorytellersDataService;
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly IRemoteTribesDataService _remoteTribesDataService;
        private readonly ILocalStorytellersDataService _localStorytellersDataService;
        private readonly IRouter _router;

        public NotificationHandlerBusinessLogic(IRouter router,
            IRemoteStoriesDataService remoteStoriesDataService,
            IRemoteStorytellersDataService remoteStorytellersDataService,
            IRemoteNotificationsDataService remoteNotificationsDataService,
            IRemoteTribesDataService remoteTribesDataService,
            ILocalStorytellersDataService localStorytellersDataService)
        {
            _router = router;
            _remoteStorytellersDataService = remoteStorytellersDataService;
            _remoteStoriesDataService = remoteStoriesDataService;
            _remoteNotificationsDataService = remoteNotificationsDataService;
            _remoteTribesDataService = remoteTribesDataService;
            _localStorytellersDataService = localStorytellersDataService;
        }

        public async Task<bool> RejectFriendshipRequestAsync(int notificationId, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.RejectFriendshipRequestAsync(dto.Id, notificationId)
                .ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptFriendshipRequestAsync(int notificationId, StorytellerDTO dto)
        {
            var result = await _remoteStorytellersDataService.AcceptFriendshipRequestAsync(dto.Id, notificationId)
                .ConfigureAwait(false);
            if (!result.IsSuccess)
                return result.IsSuccess;

            dto.FriendshipStatus = result.Data;
            await _localStorytellersDataService.SaveAsync(dto).ConfigureAwait(false);

            return result.IsSuccess;
        }

        public async Task<bool> RejectTribeInvitationAsync(int notificationId, TribeDTO dto)
        {
            var result = await _remoteTribesDataService.RejectTribeInvitationAsync(dto.Id, notificationId)
                .ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptTribeInvitationAsync(int notificationId, TribeDTO dto)
        {
            var result = await _remoteTribesDataService.AcceptTribeInvitationAsync(dto.Id, notificationId)
                .ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> NavigateStory(int notificationId, StoryDTO story, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId)
                .ConfigureAwait(false);
            _router.NavigateViewStory(view, story);
            return result.IsSuccess;
        }

        public async Task<bool> NavigatePlaylist(int notificationId, PlaylistDTO playlist, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId)
                .ConfigureAwait(false);
            _router.NavigateViewPlaylist(view, playlist,(item, state) => {});
            return result.IsSuccess;
        }

        public async Task<bool> NavigateEvent(int notificationId, EventDTO eventDTO, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId)
                .ConfigureAwait(false);
            _router.NavigateViewEvent(view, eventDTO, (item, state) => {});
            return result.IsSuccess;
        }

        public async Task<bool> RejectStoryRequestRequestAsync(int notificationId, StoryRequestDTO dto)
        {
            var result = await _remoteStoriesDataService.RejectStoryRequestAsync(dto.Id, notificationId)
                .ConfigureAwait(false);
            return result.IsSuccess;
        }

        public async Task<bool> AcceptStoryRequestRequest(int notificationId, StoryRequestDTO dto, IView view)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId)
                .ConfigureAwait(false);
            _router.NavigateRecordStory(view, dto);
            return result.IsSuccess;
        }

        public async Task<bool> HandleNotification(int notificationId)
        {
            var result = await _remoteNotificationsDataService.HandleNotificationAsync(notificationId)
                .ConfigureAwait(false);
            return result.IsSuccess;
        }
    }
}