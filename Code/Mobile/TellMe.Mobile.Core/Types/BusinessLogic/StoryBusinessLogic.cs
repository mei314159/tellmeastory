using System.Collections.Generic;
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
        protected readonly ILocalStoriesDataService LocalStoriesService;
        protected readonly IRemoteStoriesDataService RemoteStoriesDataService;
        protected readonly IRouter Router;
        protected readonly List<StoryDTO> Stories = new List<StoryDTO>();

        public StoryBusinessLogic(
            IRemoteStoriesDataService remoteStoriesDataService,
            IRouter router, ILocalStoriesDataService localStoriesService)
        {
            RemoteStoriesDataService = remoteStoriesDataService;
            Router = router;
            LocalStoriesService = localStoriesService;
        }

        public IStoryView View { get; set; }

        public void AddToPlaylist(StoryDTO story)
        {
            Router.NavigatePlaylists(this.View, PlaylistViewMode.SelectOne,
                async (dismissable, playlist) => await AddToPlaylistAsync(story, dismissable, playlist).ConfigureAwait(false));
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