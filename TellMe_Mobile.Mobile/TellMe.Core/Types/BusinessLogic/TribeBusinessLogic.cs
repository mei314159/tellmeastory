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

namespace TellMe.Core.Types.BusinessLogic
{
    public class TribeBusinessLogic
    {
        private RemoteStoriesDataService _remoteStoriesService;
        private RemoteTribesDataService _remoteTribesService;
        private LocalStoriesDataService _localStoriesService;
        private LocalTribesDataService _localTribesService;
        private ITribeView _view;
        private IRouter _router;
        readonly List<StoryDTO> stories = new List<StoryDTO>();

        public TribeBusinessLogic(RemoteStoriesDataService remoteStoriesService, RemoteTribesDataService remoteTribesService, ITribeView view, IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _remoteTribesService = remoteTribesService;
            _localStoriesService = new LocalStoriesDataService();
            _localTribesService = new LocalTribesDataService();
            _view = view;
            _router = router;
        }

        public async Task LoadStoriesAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                stories.Clear();
            }

            var result = await _remoteStoriesService.GetStoriesAsync(_view.Tribe.Id, stories.Count).ConfigureAwait(false);
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
            if (_view.Tribe == null)
            {
                var localStoryteller = await _localTribesService.GetAsync(_view.TribeId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteTribesService.GetTribeAsync(_view.TribeId).ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        _view.Tribe = result.Data;
                    }
                    else
                    {
                        result.ShowResultError(this._view);
                        return false;
                    }
                }
                else
                {
                    _view.Tribe = localStoryteller.Data;
                }
            }

            _view.DisplayTribe(_view.Tribe);
            return true;
        }

        public void SendStory()
        {
            _router.NavigateRecordStory(_view, contact: new ContactDTO
            {
                Type = ContactType.Tribe,
                TribeId = _view.Tribe.Id,
                Tribe = _view.Tribe
            });
        }

        public void ViewStory(StoryDTO story)
        {
            _router.NavigateViewStory(this._view, story);
        }

        public void TribeInfo()
        {
            _router.NavigateTribeInfo(_view, _view.Tribe, HandleTribeLeftHandler);
        }

        void HandleTribeLeftHandler(TribeDTO tribe)
        {
            this._view.TribeLeft(tribe);
        }

        public void NavigateStoryteller(StoryDTO story)
        {
            _router.NavigateStoryteller(_view, story.SenderId);
        }
    }
}
