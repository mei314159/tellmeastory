using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class RequestStoryBusinessLogic
    {
        private IRequestStoryView _view;
        private IRouter _router;
        private RemoteStoriesDataService _remoteStoriesDataService;
        private List<ContactDTO> recipientsList;
        private RequestStoryValidator _validator;
        public RequestStoryBusinessLogic(IRequestStoryView _view, IRouter _router, RemoteStoriesDataService remoteStoriesDataService)
        {
            this._view = _view;
            this._router = _router;
            this._remoteStoriesDataService = remoteStoriesDataService;
            this._validator = new RequestStoryValidator();
            this.recipientsList = new List<ContactDTO>();
        }

        public void ChooseRecipients()
        {
            _router.NavigateChooseRecipients(_view, RecipientsSelected);
        }

        void RecipientsSelected(ICollection<ContactDTO> selectedItems)
        {
            recipientsList.Clear();
            recipientsList.AddRange(selectedItems);
            this._view.SendButton.Enabled = selectedItems.Count > 0;
            this._view.DisplayRecipients(selectedItems);
        }

        public async Task SendAsync()
        {
            this._view.SendButton.Enabled = false;
            var title = this._view.StoryName.Text;

            var dto = new StoryRequestDTO
            {
                Title = title,
                ReceiverIds = recipientsList.Select(x => x.UserId).ToArray()
            };

            var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                var result = await this._remoteStoriesDataService.RequestStoryAsync(dto).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    this._view.ShowSuccessMessage("Story successfully requested", () => _view.Close(result.Data));
                }
                else
                {
                    result.ShowResultError(this._view);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this._view);
            }

            _view.InvokeOnMainThread(() => this._view.SendButton.Enabled = true);
        }
    }
}
