using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SignupBusinessLogic : ISignupBusinessLogic
    {
        private readonly IRemoteAccountDataService _remoteAccountDataService;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRouter _router;
        private readonly SignUpValidator _signUpValidator;
        private readonly SignInValidator _signInValidator;

        public SignupBusinessLogic(
            IRouter router,
            IRemoteAccountDataService remoteAccountDataService,
            ILocalAccountService localAccountService,
            SignUpValidator signUpValidator, SignInValidator signInValidator)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _localAccountService = localAccountService;
            _signUpValidator = signUpValidator;
            _signInValidator = signInValidator;
        }

        public ISignUpView View { get; set; }

        public void Init()
        {
        }

        public async Task SignUpAsync()
        {
            var validationResult = await _signUpValidator
                .ValidateAsync(View)
                .ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                validationResult.ShowValidationResult(this.View);
                return;
            }

            var dto = new SignUpDTO
            {
                UserName = this.View.UserNameField.Text,
                Email = this.View.EmailField.Text,
                FullName = this.View.FullNameField.Text,
                Password = this.View.PasswordField.Text,
                ConfirmPassword = this.View.ConfirmPasswordField.Text,
            };

            var result = await this._remoteAccountDataService.SignUpAsync(dto)
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                result.ShowResultError(this.View);
                return;
            }

            View.InvokeOnMainThread(async () => await SignInAsync(dto).ConfigureAwait(false));
        }

        private async Task SignInAsync(SignUpDTO dto)
        {
            var validationResult = await _signInValidator
                .ValidateAsync(View)
                .ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                validationResult.ShowValidationResult(this.View);
                return;
            }

            var result = await _remoteAccountDataService
                .SignInAsync(dto.Email, dto.Password)
                .ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                result.ShowResultError(this.View);
                return;
            }

            result.Data.AuthDate = DateTime.UtcNow;
            _localAccountService.SaveAuthInfo(result.Data);
            _router.NavigateSetProfilePicture(View);
        }
    }
}