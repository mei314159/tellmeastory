using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class RequestStoryBusinessLogic : IRequestStoryBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly RequestStoryValidator _validator;

        public RequestStoryBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, RequestStoryValidator validator)
        {
            this._remoteStoriesDataService = remoteStoriesDataService;
            _validator = validator;
        }

        public IRequestStoryView View { get; set; }

        public async Task SendAsync()
        {
            this.View.SendButton.Enabled = false;
            var title = this.View.StoryTitle.Text;

            var dto = new RequestStoryDTO
            {
                Requests = View.Recipients.Select(x => new StoryRequestDTO
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
                    this.View.ShowSuccessMessage("Story successfully requested", () => View.Close(result.Data));
                }
                else
                {
                    result.ShowResultError(this.View);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this.View);
            }

            View.InvokeOnMainThread(() => this.View.SendButton.Enabled = true);
        }
    }
}
