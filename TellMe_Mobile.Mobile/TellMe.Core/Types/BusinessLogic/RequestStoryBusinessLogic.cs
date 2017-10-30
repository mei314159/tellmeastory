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
        private RequestStoryValidator _validator;

        public RequestStoryBusinessLogic(IRequestStoryView _view, IRouter _router, RemoteStoriesDataService remoteStoriesDataService)
        {
            this._view = _view;
            this._router = _router;
            this._remoteStoriesDataService = remoteStoriesDataService;
            this._validator = new RequestStoryValidator();
        }

        public async Task SendAsync()
        {
            this._view.SendButton.Enabled = false;
            var title = this._view.StoryTitle.Text;

            var dto = new RequestStoryDTO
            {
                Requests = _view.Recipients.Select(x => new StoryRequestDTO
                {
                    Title = title,
                    UserId = x.Type == ContactType.User ? x.User.Id : null,
                    TribeId = x.Type == ContactType.Tribe ? x.Tribe.Id : (int?)null
                }).ToList()
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
