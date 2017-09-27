using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SignupPhoneBusinessLogic
    {
        private readonly RemoteAccountDataService _remoteAccountDataService;
        private readonly AccountService _accountService;
        private readonly ISignUpPhoneView _view;
        private readonly IRouter _router;
        private readonly SignUpPhoneValidator signUpPhoneValidator;
        private readonly SignInPhoneValidator signInPhoneValidator;

        public SignupPhoneBusinessLogic(IRouter router, RemoteAccountDataService remoteAccountDataService, AccountService accountService, ISignUpPhoneView view)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _accountService = accountService;
            _view = view;
            signUpPhoneValidator = new SignUpPhoneValidator();
            signInPhoneValidator = new SignInPhoneValidator();
        }

        public async Task SignUpAsync()
        {
            var validationResult = await signUpPhoneValidator
                .ValidateAsync(_view)
                .ConfigureAwait(false);

            if (validationResult.IsValid)
            {
                var dto = new SignUpPhoneDTO
                {
                    PhoneNumber = _view.PhoneNumberField.Text
                };

                var result = await this._remoteAccountDataService.SignUpPhoneAsync(dto)
                                       .ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    validationResult = await signInPhoneValidator
                        .ValidateAsync(_view)
                        .ConfigureAwait(false);

                    if (validationResult.IsValid)
                    {
                        var authResult = await _remoteAccountDataService
                            .SignInPhoneAsync(dto.PhoneNumber, _view.ConfirmationCode)
                            .ConfigureAwait(false);
                        if (authResult.IsSuccess)
                        {
                            authResult.Data.AuthDate = DateTime.UtcNow;
                            _accountService.SaveAuthInfo(authResult.Data);
                            _router.NavigateImportContacts();
                            return;
                        }
                    }
                }
                else
                {
                    result.ShowResultError(this._view);
                }
            }

            validationResult.ShowValidationResult(this._view);
        }
    }
}
