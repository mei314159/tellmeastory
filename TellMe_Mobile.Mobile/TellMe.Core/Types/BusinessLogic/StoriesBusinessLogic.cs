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
        private LocalStoriesDataService _localStoriesService;
        private IStoriesListView _view;
        private IRouter _router;
        private RequestStoryValidator _validator;
        readonly List<StoryDTO> stories = new List<StoryDTO>();

        public StoriesBusinessLogic(RemoteStoriesDataService remoteStoriesService, IStoriesListView view, IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _localStoriesService = new LocalStoriesDataService();
            _view = view;
            _router = router;
            _validator = new RequestStoryValidator();
        }

        public async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            var localContacts = await _localStoriesService.GetAllAsync().ConfigureAwait(false);
            if (localContacts.Expired || forceRefresh || clearCache)
            {
                if (forceRefresh)
                {
                    stories.Clear();
                }

                if (clearCache){
                    await _localStoriesService.DeleteAllAsync().ConfigureAwait(false);
                }
                var result = await _remoteStoriesService.GetStoriesAsync(stories.Count).ConfigureAwait(false);
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
            }
            else
            {
                stories.Clear();
                stories.AddRange(localContacts.Data);
            }

            this._view.DisplayStories(stories.OrderByDescending(x => x.UpdateDateUtc).ToList());
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

        void RequestStoryRecipientSelectedEventHandler(StorytellerDTO selectedStoryteller)
        {
            _router.NavigateRequestStory(this._view, selectedStoryteller);
        }
    }
}
