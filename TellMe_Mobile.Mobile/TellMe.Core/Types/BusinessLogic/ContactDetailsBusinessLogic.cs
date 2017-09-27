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
        private IContactDetailsView _view;

        public ContactDetailsBusinessLogic(RemoteStoriesDataService remoteStoriesService, IContactDetailsView view)
        {
            _remoteStoriesService = remoteStoriesService;
            _localContactsService = new LocalContactsDataService();
            _view = view;
        }

        public void LoadContactDetails()
        {
            this._view.DisplayContactDetails(_view.ContactDTO);
        }

        public void RequestStory()
        {
            this._view.DisplayStoryDetailsPrompt();
        }

        public async Task RequestStoryAsync(string title, string description)
        {
            var dto = new StoryRequestDTO
            {
                Title = title,
                Description = description,
                ReceiverId = _view.ContactDTO.UserId
            };

            var result = await this._remoteStoriesService.RequestStoryAsync(dto).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                _view.ShowSuccessMessage("Story requested.");
            }
            else
            {
                result.ShowResultError(this._view);
            }
        }
    }
}
