using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class SigninBusinessLogic : ISigninBusinessLogic
    {
        private readonly IRemoteAccountDataService _remoteAccountDataService;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRouter _router;
        private readonly SignInValidator _signInValidator;

        public SigninBusinessLogic(
            IRouter router,
            IRemoteAccountDataService remoteAccountDataService,
            ILocalAccountService localAccountService,
            SignInValidator signInValidator)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _localAccountService = localAccountService;
            _signInValidator = signInValidator;
        }

        public ISignInView View { get; set; }

        public void Init()
        {
        }

        public async Task SignInAsync()
        {
            var validationResult = await _signInValidator
                .ValidateAsync(View)
                .ConfigureAwait(false);

            if (validationResult.IsValid)
            {
                var result = await _remoteAccountDataService
                    .SignInAsync(View.EmailField.Text, View.PasswordField.Text)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    _localAccountService.SaveAuthInfo(result.Data);
                    _router.NavigateMain();
                    return;
                }
                result.ShowResultError(this.View);
            }

            validationResult.ShowValidationResult(this.View);
        }
    }
}