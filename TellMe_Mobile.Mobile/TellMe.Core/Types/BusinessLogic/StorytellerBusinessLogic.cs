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
    public class StorytellerBusinessLogic : IStorytellerBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesService;
        private readonly IRemoteStorytellersDataService _remoteStorytellesService;
        private readonly ILocalStoriesDataService _localStoriesService;
        private readonly ILocalStorytellersDataService _localStorytellesService;
        private readonly ILocalAccountService _localLocalAccountService;
        private readonly IRouter _router;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();

        public StorytellerBusinessLogic(IRemoteStoriesDataService remoteStoriesService,
            IRemoteStorytellersDataService remoteStorytellesService,
            IRouter router,
            ILocalStoriesDataService localStoriesService,
            ILocalStorytellersDataService localStorytellesService,
            ILocalAccountService localLocalAccountService)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteStorytellesService = remoteStorytellesService;
            _router = router;
            _localStoriesService = localStoriesService;
            _localStorytellesService = localStorytellesService;
            _localLocalAccountService = localLocalAccountService;
        }

        public IStorytellerView View { get; set; }

        public async Task LoadStoriesAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                _stories.Clear();
            }

            var result = await _remoteStoriesService
                .GetStoriesAsync(View.Storyteller.Id, forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc)
                .ConfigureAwait(false);
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

        public async Task<bool> InitAsync()
        {
            if (View.Storyteller == null)
            {
                var localStoryteller =
                    await _localStorytellesService.GetAsync(View.StorytellerId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteStorytellesService.GetByIdAsync(View.StorytellerId)
                        .ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        View.Storyteller = result.Data;
                    }
                    else
                    {
                        result.ShowResultError(this.View);
                        return false;
                    }
                }
                else
                {
                    View.Storyteller = localStoryteller.Data;
                }
            }

            View.DisplayStoryteller(View.Storyteller);
            return true;
        }

        public void SendStory()
        {
            _router.NavigateRecordStory(View, contact: new ContactDTO
            {
                Type = ContactType.User,
                UserId = View.Storyteller.Id,
                User = View.Storyteller
            });
        }

        public void RequestStory()
        {
            _router.NavigateRequestStory(this.View, new[]
            {
                new ContactDTO
                {
                    Type = ContactType.User,
                    UserId = View.Storyteller.Id,
                    User = View.Storyteller
                }
            });
        }

        public void ViewStory(StoryDTO story)
        {
            _router.NavigateViewStory(this.View, story);
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
            if (story.SenderId != _localLocalAccountService.GetAuthInfo().UserId)
            {
                _router.NavigateStoryteller(View, story.SenderId);
            }
        }
    }
}