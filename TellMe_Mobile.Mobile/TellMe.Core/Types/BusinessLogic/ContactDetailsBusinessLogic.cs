using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class ContactDetailsBusinessLogic
    {
        private RemoteStoriesDataService _remoteStoriesService;
        private LocalContactsDataService _localContactsService;
        private LocalStoriesDataService _localStoriesService;
        private IContactDetailsView _view;

        public ContactDetailsBusinessLogic(RemoteStoriesDataService remoteStoriesService, IContactDetailsView view)
        {
            _remoteStoriesService = remoteStoriesService;
            _localContactsService = new LocalContactsDataService();
            _localStoriesService = new LocalStoriesDataService();
            _view = view;
        }

        public async Task LoadContactDetails(bool forceRefresh = false)
        {
            this._view.DisplayContactDetails(_view.ContactDTO);

            ICollection<StoryDTO> stories;
            var localContacts = await _localStoriesService.GetAllAsync(this._view.ContactDTO.UserId).ConfigureAwait(false);
            if (localContacts.Expired || forceRefresh)
            {
                var result = await _remoteStoriesService.GetStoriesAsync(this._view.ContactDTO.UserId);
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
    }
}
