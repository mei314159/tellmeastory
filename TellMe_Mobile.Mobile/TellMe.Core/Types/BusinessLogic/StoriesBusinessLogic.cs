using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class StoriesBusinessLogic
    {
        private RemoteStoriesDataService _remoteStoriesService;
        private RemoteNotificationsDataService _remoteNotificationsService;
        private LocalStoriesDataService _localStoriesService;
        private IStoriesListView _view;
        private IRouter _router;
        private RequestStoryValidator _validator;
        readonly List<StoryDTO> stories = new List<StoryDTO>();

        public StoriesBusinessLogic(
            RemoteStoriesDataService remoteStoriesService,
            RemoteNotificationsDataService remoteNotificationsService,
            IStoriesListView view,
            IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteNotificationsService = remoteNotificationsService;
            _localStoriesService = new LocalStoriesDataService();
            _view = view;
            _router = router;
            _validator = new RequestStoryValidator();
        }

        public async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            var localStories = await _localStoriesService.GetAllAsync().ConfigureAwait(false);
            //if (localStories.Expired || forceRefresh || clearCache)
            //{
            if (forceRefresh)
            {
                stories.Clear();
            }

            if (clearCache)
            {
                await _localStoriesService.DeleteAllAsync().ConfigureAwait(false);
            }
            var result = await _remoteStoriesService.GetStoriesAsync(forceRefresh ? null : stories.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
            //}
            //else
            //{
            //    stories.Clear();
            //    stories.AddRange(localStories.Data);
            //}

            this._view.DisplayStories(stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }


        public void SendStory()
        {
            _router.NavigateRecordStory(_view);
        }

        public void RequestStory()
        {
            _router.NavigateChooseRecipients(_view, RequestStoryRecipientSelectedEventHandler, false);
        }

        public void AccountSettings()
        {
            _router.NavigateAccountSettings(_view);
        }

        public void ShowStorytellers()
        {
            _router.NavigateStorytellers(_view);
        }

        public void NotificationsCenter()
        {
            _router.NavigateNotificationsCenter(_view);
        }

        void RequestStoryRecipientSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            _router.NavigateRequestStory(this._view, selectedContacts);
        }

        public void ViewStory(StoryDTO story, bool goToComments = false)
        {
            _router.NavigateViewStory(this._view, story, goToComments);
        }

        public void NavigateStoryteller(StoryDTO story)
        {
            _router.NavigateStoryteller(_view, story.SenderId);
        }

        public void ViewReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe)
        {
            if (receiver.TribeId != null)
            {
                _router.NavigateTribe(_view, receiver.TribeId.Value, onRemoveTribe);
            }
            else
            {
                _router.NavigateStoryteller(_view, receiver.UserId);
            }
        }

        public async Task LoadActiveNotificationsCountAsync()
        {
            var result = await _remoteNotificationsService.GetActiveNotificationsCountAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this._view.DisplayNotificationsCount(result.Data);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
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
