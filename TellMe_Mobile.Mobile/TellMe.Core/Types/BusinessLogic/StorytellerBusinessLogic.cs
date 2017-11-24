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
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly IRemoteStorytellersDataService _remoteStorytellesDataService;
        private readonly ILocalStoriesDataService _localStoriesDataService;
        private readonly ILocalStorytellersDataService _localStorytellesDataService;
        private readonly ILocalAccountService _localLocalAccountService;
        private readonly IRouter _router;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();

        public StorytellerBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService,
            IRemoteStorytellersDataService remoteStorytellesDataService,
            IRouter router,
            ILocalStoriesDataService localStoriesDataService,
            ILocalStorytellersDataService localStorytellesDataService,
            ILocalAccountService localLocalAccountService)
        {
            _remoteStoriesDataService = remoteStoriesDataService;
            _remoteStorytellesDataService = remoteStorytellesDataService;
            _router = router;
            _localStoriesDataService = localStoriesDataService;
            _localStorytellesDataService = localStorytellesDataService;
            _localLocalAccountService = localLocalAccountService;
        }

        public IStorytellerView View { get; set; }

        public async Task LoadStoriesAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                _stories.Clear();
            }

            var result = await _remoteStoriesDataService
                .GetStoriesAsync(View.Storyteller.Id, forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localStoriesDataService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
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
                    await _localStorytellesDataService.GetAsync(View.StorytellerId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteStorytellesDataService.GetByIdAsync(View.StorytellerId)
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
            var contacts = new[]
            {
                new ContactDTO
                {
                    Type = ContactType.User,
                    UserId = View.Storyteller.Id,
                    User = View.Storyteller
                }
            };
            _router.NavigatePrepareStoryRequest(this.View, contacts, CreateStoryRequestAsync);
        }

        private async void CreateStoryRequestAsync(RequestStoryDTO dto, ICollection<ContactDTO> recipients)
        {
            var overlay = this.View.DisableInput();
            var result = await this._remoteStoriesDataService.RequestStoryAsync(dto, recipients).ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Story successfully requested");
            }
            else
            {
                result.ShowResultError(this.View);
            }  
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
                ? await _remoteStoriesDataService.DislikeAsync(story.Id).ConfigureAwait(false)
                : await _remoteStoriesDataService.LikeAsync(story.Id).ConfigureAwait(false);

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