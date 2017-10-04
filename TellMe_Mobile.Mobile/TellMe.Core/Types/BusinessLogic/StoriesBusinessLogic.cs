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
        private StoryRequestValidator _validator;

        public StoriesBusinessLogic(RemoteStoriesDataService remoteStoriesService, IStoriesListView view, IRouter router)
        {
            _remoteStoriesService = remoteStoriesService;
            _localContactsService = new LocalContactsDataService();
            _localStoriesService = new LocalStoriesDataService();
            _view = view;
            _router = router;
            _validator = new StoryRequestValidator();
        }

        public async Task LoadStories(bool forceRefresh = false)
        {
            ICollection<StoryDTO> stories;
            var localContacts = await _localStoriesService.GetAllAsync().ConfigureAwait(false);
            if (localContacts.Expired || forceRefresh)
            {
                var result = await _remoteStoriesService.GetStoriesAsync();
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

            this._view.DisplayStories(stories.OrderByDescending(x => x.RequestDateUtc).ToList());
        }

        public void SendStory()
        {
            _router.NavigateRecordStory(this._view);
        }

        public void RequestStory()
        {
            _router.NavigateRequestStory(this._view);
        }

        //public void RequestStory()
        //{
        //    this._view.DisplayStoryDetailsPrompt();
        //}

        //public async Task RequestStoryAsync(string title, string description)
        //{

        //    var dto = new StoryRequestDTO
        //    {
        //        Title = title,
        //        Description = description,
        //        ReceiverId = _view.ContactDTO.UserId
        //    };

        //    var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
        //    if (validationResult.IsValid)
        //    {
        //        var result = await this._remoteStoriesService.RequestStoryAsync(dto).ConfigureAwait(false);
        //        if (result.IsSuccess)
        //        {
        //            await this.LoadContactDetails(true).ConfigureAwait(false);
        //            _view.ShowSuccessMessage("Story requested.");
        //        }
        //        else
        //        {
        //            result.ShowResultError(this._view);
        //        }
        //    }
        //    else
        //    {
        //        validationResult.ShowValidationResult(this._view);
        //    }
        //}
    }
}
