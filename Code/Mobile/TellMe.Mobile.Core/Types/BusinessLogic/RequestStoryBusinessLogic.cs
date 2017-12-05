using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class RequestStoryBusinessLogic : IRequestStoryBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly RequestStoryValidator _validator;
        private readonly ILocalAccountService _localAccountService;

        public RequestStoryBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService,
            RequestStoryValidator validator, ILocalAccountService localAccountService)
        {
            this._remoteStoriesDataService = remoteStoriesDataService;
            _validator = validator;
            _localAccountService = localAccountService;
        }

        public IRequestStoryView View { get; set; }

        public async Task CreateStoryRequest()
        {
            this.View.SendButton.Enabled = false;
            var title = this.View.StoryTitle.Text;

            var dto = new RequestStoryDTO
            {
                Title = title
            };

            var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                View.Close(dto, View.Recipients);
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