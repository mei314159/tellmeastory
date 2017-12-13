using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class StoriesTableBusinessLogic : IStoriesTableBusinessLogic
    {
        protected readonly ILocalStoriesDataService LocalStoriesService;
        protected readonly IRemoteStoriesDataService RemoteStoriesDataService;
        protected readonly IRouter Router;
        protected readonly List<StoryDTO> Stories = new List<StoryDTO>();

        public StoriesTableBusinessLogic(
            IRemoteStoriesDataService remoteStoriesDataService,
            IRouter router, ILocalStoriesDataService localStoriesService)
        {
            RemoteStoriesDataService = remoteStoriesDataService;
            Router = router;
            LocalStoriesService = localStoriesService;
        }

        public IStoriesTableView View { get; set; }

        public virtual async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                Stories.Clear();
            }

            if (clearCache)
            {
                await LocalStoriesService.DeleteAllAsync().ConfigureAwait(false);
            }
            var result = await RemoteStoriesDataService
                .GetStoriesAsync(forceRefresh ? null : Stories.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await LocalStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                Stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayStories(Stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public void ViewStory(StoryDTO story, bool goToComments = false)
        {
            Router.NavigateViewStory(this.View, story, goToComments);
        }

        public void NavigateStoryteller(string userId)
        {
            Router.NavigateStoryteller(View, userId);
        }

        public void NavigateTribe(int tribeId, TribeLeftHandler onRemoveTribe)
        {
            Router.NavigateTribe(View, tribeId, onRemoveTribe);
        }

        public void NavigateReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe)
        {
            if (receiver.TribeId != null)
            {
                this.NavigateTribe(receiver.TribeId.Value, onRemoveTribe);
            }
            else
            {
                this.NavigateStoryteller(receiver.UserId);
            }
        }

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

        public async Task LikeButtonTouchedAsync(StoryDTO story)
        {
            var liked = story.Liked;
            var likeCount = story.LikesCount;
            story.Liked = !liked;
            story.LikesCount = liked ? likeCount - 1 : likeCount + 1;
            App.Instance.StoryLikeChanged(story);

            var result = liked
                ? await RemoteStoriesDataService.DislikeAsync(story.Id).ConfigureAwait(false)
                : await RemoteStoriesDataService.LikeAsync(story.Id).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                story.Liked = liked;
                story.LikesCount = likeCount;
                App.Instance.StoryLikeChanged(story);
            }
        }

        public virtual Task<bool> InitAsync()
        {
            return Task.FromResult(true);
        }
    }
}