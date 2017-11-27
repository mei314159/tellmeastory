using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
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