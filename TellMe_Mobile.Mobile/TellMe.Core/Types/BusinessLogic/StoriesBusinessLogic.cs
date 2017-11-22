using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class StoriesBusinessLogic : IStoriesBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesService;
        private readonly IRemoteNotificationsDataService _remoteNotificationsService;
        private readonly ILocalStoriesDataService _localStoriesService;
        private readonly IRouter _router;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();

        public StoriesBusinessLogic(
            IRemoteStoriesDataService remoteStoriesService,
            IRemoteNotificationsDataService remoteNotificationsService,
            IRouter router, ILocalStoriesDataService localStoriesService)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteNotificationsService = remoteNotificationsService;
            _router = router;
            _localStoriesService = localStoriesService;
        }

        public IStoriesListView View { get; set; }

        public async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                _stories.Clear();
            }

            if (clearCache)
            {
                await _localStoriesService.DeleteAllAsync().ConfigureAwait(false);
            }
            var result = await _remoteStoriesService
                .GetStoriesAsync(forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                _stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayStories(_stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }


        public void SendStory()
        {
            _router.NavigateRecordStory(View);
        }

        public void RequestStory()
        {
            _router.NavigateChooseRecipients(View, RequestStoryRecipientSelectedEventHandler, false);
        }

        public void AccountSettings()
        {
            _router.NavigateAccountSettings(View);
        }

        public void ShowStorytellers()
        {
            _router.NavigateStorytellers(View);
        }

        public void NotificationsCenter()
        {
            _router.NavigateNotificationsCenter(View);
        }

        private void RequestStoryRecipientSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            _router.NavigateRequestStory(this.View, selectedContacts);
        }

        public void ViewStory(StoryDTO story, bool goToComments = false)
        {
            _router.NavigateViewStory(this.View, story, goToComments);
        }

        public void NavigateStoryteller(StoryDTO story)
        {
            _router.NavigateStoryteller(View, story.SenderId);
        }

        public void ViewReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe)
        {
            if (receiver.TribeId != null)
            {
                _router.NavigateTribe(View, receiver.TribeId.Value, onRemoveTribe);
            }
            else
            {
                _router.NavigateStoryteller(View, receiver.UserId);
            }
        }

        public async Task LoadActiveNotificationsCountAsync()
        {
            var result = await _remoteNotificationsService.GetActiveNotificationsCountAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.DisplayNotificationsCount(result.Data);
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
                ? await _remoteStoriesService.DislikeAsync(story.Id).ConfigureAwait(false)
                : await _remoteStoriesService.LikeAsync(story.Id).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                story.Liked = liked;
                story.LikesCount = likeCount;
                App.Instance.StoryLikeChanged(story);
            }
        }
    }
}