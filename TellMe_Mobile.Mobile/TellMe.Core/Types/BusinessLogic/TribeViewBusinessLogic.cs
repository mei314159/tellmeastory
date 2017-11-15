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
    public class TribeViewBusinessLogic : ITribeViewBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesService;
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly ILocalStoriesDataService _localStoriesService;
        private readonly ILocalTribesDataService _localTribesService;
        private readonly IRouter _router;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();

        public TribeViewBusinessLogic(IRemoteStoriesDataService remoteStoriesService,
            IRemoteTribesDataService remoteTribesService, IRouter router,
            ILocalStoriesDataService localStoriesService, ILocalTribesDataService localTribesService)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteTribesService = remoteTribesService;
            _router = router;
            _localStoriesService = localStoriesService;
            _localTribesService = localTribesService;
        }

        public ITribeView View { get; set; }

        public async Task LoadStoriesAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                _stories.Clear();
            }

            var result = await _remoteStoriesService
                .GetStoriesAsync(View.Tribe.Id, forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc)
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
            if (View.Tribe == null)
            {
                var localStoryteller = await _localTribesService.GetAsync(View.TribeId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteTribesService.GetTribeAsync(View.TribeId).ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        View.Tribe = result.Data;
                    }
                    else
                    {
                        result.ShowResultError(this.View);
                        return false;
                    }
                }
                else
                {
                    View.Tribe = localStoryteller.Data;
                }
            }

            View.DisplayTribe(View.Tribe);
            return true;
        }

        public void SendStory()
        {
            _router.NavigateRecordStory(View, contact: new ContactDTO
            {
                Type = ContactType.Tribe,
                TribeId = View.Tribe.Id,
                Tribe = View.Tribe
            });
        }

        public void ViewStory(StoryDTO story)
        {
            _router.NavigateViewStory(this.View, story);
        }

        public void TribeInfo()
        {
            _router.NavigateTribeInfo(View, View.Tribe, HandleTribeLeftHandler);
        }

        private void HandleTribeLeftHandler(TribeDTO tribe)
        {
            this.View.TribeLeft(tribe);
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