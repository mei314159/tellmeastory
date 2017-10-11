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
    public class SignupBusinessLogic
    {
        private readonly RemoteAccountDataService _remoteAccountDataService;
        private readonly AccountService _accountService;
        private readonly ISignUpView _view;
        private readonly IRouter _router;
        private readonly SignUpValidator _signUpValidator;
        private readonly SignInValidator _signInValidator;

        public SignupBusinessLogic(
            IRouter router,
            RemoteAccountDataService remoteAccountDataService,
            AccountService accountService,
            ISignUpView view)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _accountService = accountService;
            _view = view;
            _signUpValidator = new SignUpValidator();
            _signInValidator = new SignInValidator();
        }

        public void Init()
        {
        }

        public async Task SignUpAsync()
        {
            var validationResult = await _signUpValidator
                .ValidateAsync(_view)
                .ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                validationResult.ShowValidationResult(this._view);
                return;
            }

            var dto = new SignUpDTO
            {
                UserName = this._view.UserNameField.Text,
                Email = this._view.EmailField.Text,
                FullName = this._view.FullNameField.Text,
                Password = this._view.PasswordField.Text,
                ConfirmPassword = this._view.ConfirmPasswordField.Text,
            };

            var result = await this._remoteAccountDataService.SignUpAsync(dto)
                                   .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                result.ShowResultError(this._view);
                return;
            }

            _view.InvokeOnMainThread(async () => await SignInAsync(dto).ConfigureAwait(false));
        }

        private async Task SignInAsync(SignUpDTO dto)
        {
            var validationResult = await _signInValidator
                    .ValidateAsync(_view)
                    .ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                validationResult.ShowValidationResult(this._view);
                return;
            }

            var result = await _remoteAccountDataService
                .SignInAsync(dto.Email, dto.Password)
                .ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                result.ShowResultError(this._view);
                return;
            }

            result.Data.AuthDate = DateTime.UtcNow;
            _accountService.SaveAuthInfo(result.Data);
            _router.NavigateSetProfilePicture(_view);
        }
    }
}
