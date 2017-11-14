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
    public class StorytellerBusinessLogic
    {
        private RemoteStoriesDataService _remoteStoriesService;
        private RemoteStorytellersDataService _remoteStorytellesService;
        private LocalStoriesDataService _localStoriesService;
        private LocalStorytellersDataService _localStorytellesService;
        private IStorytellerView _view;
        private IRouter _router;
        readonly List<StoryDTO> stories = new List<StoryDTO>();

        public StorytellerBusinessLogic(RemoteStoriesDataService remoteStoriesService, RemoteStorytellersDataService remoteStorytellesService, IStorytellerView view, IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteStorytellesService = remoteStorytellesService;
            _localStoriesService = new LocalStoriesDataService();
            _localStorytellesService = new LocalStorytellersDataService();
            _view = view;
            _router = router;
        }

        public async Task LoadStoriesAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                stories.Clear();
            }

            var result = await _remoteStoriesService.GetStoriesAsync(_view.Storyteller.Id, forceRefresh ? null : stories.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
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

            this._view.DisplayStories(stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public async Task<bool> InitAsync()
        {
            if (_view.Storyteller == null)
            {
                var localStoryteller = await _localStorytellesService.GetAsync(_view.StorytellerId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteStorytellesService.GetByIdAsync(_view.StorytellerId).ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        _view.Storyteller = result.Data;
                    }
                    else
                    {
                        result.ShowResultError(this._view);
                        return false;
                    }
                }
                else
                {
                    _view.Storyteller = localStoryteller.Data;
                }
            }

            _view.DisplayStoryteller(_view.Storyteller);
            return true;
        }

        public void SendStory()
        {
            _router.NavigateRecordStory(_view, contact: new ContactDTO
            {
                Type = ContactType.User,
                UserId = _view.Storyteller.Id,
                User = _view.Storyteller
            });
        }

        public void RequestStory()
        {
            _router.NavigateRequestStory(this._view, new[] {
                new ContactDTO
                {
                    Type = ContactType.User,
                    UserId = _view.Storyteller.Id,
                    User = _view.Storyteller
                }});
        }

        public void ViewStory(StoryDTO story)
        {
            _router.NavigateViewStory(this._view, story);
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

        public void NavigateStoryteller(StoryDTO story)
        {
            if (story.SenderId != App.Instance.AuthInfo.UserId)
            {
                _router.NavigateStoryteller(_view, story.SenderId);
            }
        }
    }
}
