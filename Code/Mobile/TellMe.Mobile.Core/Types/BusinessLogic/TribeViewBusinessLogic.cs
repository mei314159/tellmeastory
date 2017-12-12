using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Shared.Contracts.Enums;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class TribeViewBusinessLogic : StoriesTableBusinessLogic, ITribeViewBusinessLogic
    {
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly ILocalTribesDataService _localTribesService;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();

        public TribeViewBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, IRouter router,
            ILocalStoriesDataService localStoriesService, IRemoteTribesDataService remoteTribesService,
            ILocalTribesDataService localTribesService) : base(remoteStoriesDataService, router, localStoriesService)
        {
            _remoteTribesService = remoteTribesService;
            _localTribesService = localTribesService;
        }

        public new ITribeView View
        {
            get => (ITribeView) base.View;
            set => base.View = value;
        }

        public override async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                _stories.Clear();
            }

            var result = await RemoteStoriesDataService
                .GetStoriesAsync(View.Tribe.Id, forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await LocalStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                _stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayStories(_stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public override async Task<bool> InitAsync()
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
            Router.NavigateRecordStory(View, contact: new ContactDTO
            {
                Type = ContactType.Tribe,
                TribeId = View.Tribe.Id,
                Tribe = View.Tribe
            });
        }

        public void ViewStory(StoryDTO story)
        {
            Router.NavigateViewStory(this.View, story);
        }

        public void TribeInfo()
        {
            Router.NavigateTribeInfo(View, View.Tribe, HandleTribeLeftHandler);
        }

        private void HandleTribeLeftHandler(TribeDTO tribe)
        {
            this.View.TribeLeft(tribe);
        }
    }
}