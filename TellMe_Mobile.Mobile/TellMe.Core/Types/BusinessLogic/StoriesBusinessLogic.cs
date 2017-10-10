using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
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
        private LocalContactsDataService _localContactsService;
        private LocalStoriesDataService _localStoriesService;
        private IStoriesListView _view;
        private IRouter _router;
        private RequestStoryValidator _validator;

        public StoriesBusinessLogic(RemoteStoriesDataService remoteStoriesService, IStoriesListView view, IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _localContactsService = new LocalContactsDataService();
            _localStoriesService = new LocalStoriesDataService();
            _view = view;
            _router = router;
            _validator = new RequestStoryValidator();
        }

        public async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            ICollection<StoryDTO> stories;
            var localContacts = await _localStoriesService.GetAllAsync().ConfigureAwait(false);
            if (localContacts.Expired || forceRefresh || clearCache)
            {
                if (clearCache){
                    await _localStoriesService.DeleteAllAsync().ConfigureAwait(false);
                }
                var result = await _remoteStoriesService.GetStoriesAsync().ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                    stories = result.Data;
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }
            else
            {
                stories = localContacts.Data;
            }

            this._view.DisplayStories(stories.OrderByDescending(x => x.UpdateDateUtc).ToList());
        }


        public void SendStory(StoryDTO requestedStory = null)
		{
            _router.NavigateRecordStory(this._view, requestedStory);
        }

        public void RequestStory()
        {
            _router.NavigateRequestStory(this._view, (requestedStories) => this.LoadStoriesAsync(true, false));
        }
    }
}
