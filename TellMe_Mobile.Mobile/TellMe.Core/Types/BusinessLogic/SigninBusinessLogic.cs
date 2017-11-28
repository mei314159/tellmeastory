using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
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