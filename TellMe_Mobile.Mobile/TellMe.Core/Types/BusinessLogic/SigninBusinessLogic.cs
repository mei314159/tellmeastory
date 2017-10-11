using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SigninBusinessLogic
    {
        private readonly RemoteAccountDataService _remoteAccountDataService;
        private readonly AccountService _accountService;
        private readonly ISignInView _view;
        private readonly IRouter _router;
        private readonly SignInValidator _signInValidator;

        public SigninBusinessLogic(
            IRouter router,
            RemoteAccountDataService remoteAccountDataService,
            AccountService accountService,
            ISignInView view)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _accountService = accountService;
            _view = view;
            _signInValidator = new SignInValidator();
        }

        public void Init()
        {
        }

        public async Task SignInAsync()
        {
            var validationResult = await _signInValidator
                .ValidateAsync(_view)
                .ConfigureAwait(false);

            if (validationResult.IsValid)
            {
                var result = await _remoteAccountDataService
                    .SignInAsync(_view.EmailField.Text, _view.PasswordField.Text)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    result.Data.AuthDate = DateTime.UtcNow;
                    _accountService.SaveAuthInfo(result.Data);
                    _router.NavigateMain();
                    return;
                }
                result.ShowResultError(this._view);
            }

            validationResult.ShowValidationResult(this._view);
        }
    }


}
