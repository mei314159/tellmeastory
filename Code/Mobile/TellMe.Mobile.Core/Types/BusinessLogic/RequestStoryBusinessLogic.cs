using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class RequestStoryBusinessLogic : IRequestStoryBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly RequestStoryValidator _validator;
        private readonly ILocalAccountService _localAccountService;

        public RequestStoryBusinessLogic(RequestStoryValidator validator, ILocalAccountService localAccountService,
            IRemoteStoriesDataService remoteStoriesDataService)
        {
            _validator = validator;
            _localAccountService = localAccountService;
            _remoteStoriesDataService = remoteStoriesDataService;
        }

        public IRequestStoryView View { get; set; }

        public async Task CreateStoryRequest()
        {
            this.View.SendButton.Enabled = false;
            var title = this.View.StoryTitle.Text;

            var dto = new RequestStoryDTO
            {
                Title = title,
                EventId = View.Event?.Id,
                Recipients = View.Recipients.Select(x => new SharedContactDTO
                {
                    TribeId = x.TribeId,
                    UserId = x.UserId,
                    Type = x.Type
                }).ToList()
            };

            var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                var result = await this._remoteStoriesDataService.RequestStoryAsync(dto)
                    .ConfigureAwait(false);
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

        public string GetUsername()
        {
            return _localAccountService.GetAuthInfo().Account.UserName;
        }
    }
}