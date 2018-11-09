using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class StoryBusinessLogic : IStoryBusinessLogic
    {
        protected readonly IRemoteStoriesDataService RemoteStoriesDataService;
        protected readonly ILocalAccountService LocalAccountService;
        protected readonly IRemoteStorytellersDataService RemoteStorytellersDataService;
        protected readonly IRouter Router;

        public StoryBusinessLogic(
            IRemoteStoriesDataService remoteStoriesDataService,
            IRouter router,
            ILocalAccountService localAccountService,
            IRemoteStorytellersDataService remoteStorytellersDataService)
        {
            RemoteStoriesDataService = remoteStoriesDataService;
            Router = router;
            LocalAccountService = localAccountService;
            RemoteStorytellersDataService = remoteStorytellersDataService;
        }

        public IStoryView View { get; set; }

        public void AddToPlaylist(StoryDTO story)
        {
            Router.NavigatePlaylists(this.View, PlaylistViewMode.SelectOne,
                async (dismissable, playlist) => await AddToPlaylistAsync(story, dismissable, playlist).ConfigureAwait(false));
        }

        public async Task UnfollowStoryTellerAsync(string senderId)
        {
            if (senderId == LocalAccountService.GetAuthInfo().UserId)
            {
                this.View.ShowErrorMessage("Error", "You can't unfollow yourself");
            }
            else
            {
                var result = await RemoteStorytellersDataService.UnfollowAsync(senderId).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    this.View.ShowSuccessMessage("Storyteller has been unfollowed");
                }
                else
                {
                    this.View.ShowErrorMessage("Error", result.ErrorMessage);
                }
            }
        }

        public async Task FlagAsObjectionable(int storyId)
        {
            var result = await RemoteStoriesDataService.FlagAsObjectionableAsync(storyId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("The story was flagged as objectionable");
            }
            else
            {
                this.View.ShowErrorMessage("Error", result.ErrorMessage);
            }
        }

        public async Task UnflagAsObjectionable(int storyId)
        {
            var result = await RemoteStoriesDataService.UnflagAsObjectionableAsync(storyId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("The story was unflagged as objectionable");
            }
            else
            {
                this.View.ShowErrorMessage("Error", result.ErrorMessage);
            }
        }

        private async Task AddToPlaylistAsync(StoryDTO story, IDismissable dismissable, PlaylistDTO playlist)
        {
            var result = await RemoteStoriesDataService.AddToPlaylistAsync(story.Id, playlist.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Story successfully added to the playlist", dismissable.Dismiss);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}