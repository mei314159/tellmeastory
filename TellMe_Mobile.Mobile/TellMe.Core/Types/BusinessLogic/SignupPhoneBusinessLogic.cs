using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SignupPhoneBusinessLogic
    {
        private readonly AccountService _accountService;
        private readonly ISignUpPhoneView _view;
        private readonly IRouter _router;
        private readonly SignUpPhoneValidator signUpPhoneValidator;
        private readonly SignInPhoneValidator signInPhoneValidator;

        public SignupPhoneBusinessLogic(IRouter router, AccountService accountService, ISignUpPhoneView view)
        {
            _router = router;
            _accountService = accountService;
            _view = view;
            signUpPhoneValidator = new SignUpPhoneValidator();
            signInPhoneValidator = new SignInPhoneValidator();
        }

        public async Task SignUpAsync()
        {
            var validationResult = await signUpPhoneValidator.ValidateAsync(_view).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                var dto = new SignUpPhoneDTO
                {
                    PhoneNumber = _view.PhoneNumberField.Text
                };

                var result = await this._accountService.SignUpPhoneAsync(dto).ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    validationResult = await signInPhoneValidator
                        .ValidateAsync(_view)
                        .ConfigureAwait(false);
                    if (validationResult.IsValid)
                    {
                        var authResult = await _accountService
                            .SignInPhoneAsync(dto.PhoneNumber, _view.ConfirmationCode).ConfigureAwait(false);
                        if (authResult.IsSuccess)
                        {
                            App.Instance.DataStorage.AuthInfo = authResult.Data;
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
