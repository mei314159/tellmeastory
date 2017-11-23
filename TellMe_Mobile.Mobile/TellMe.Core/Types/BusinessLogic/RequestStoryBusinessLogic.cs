using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
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