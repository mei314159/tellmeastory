using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Providers;
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
        private readonly ILocaleProvider _localeProvider;
        private readonly IRouter _router;
        private readonly SignUpPhoneValidator signUpPhoneValidator;
        private readonly SignInPhoneValidator signInPhoneValidator;
        private string selectedCountryCode;

        public SignupPhoneBusinessLogic(
            IRouter router,
            RemoteAccountDataService remoteAccountDataService,
            AccountService accountService,
            ILocaleProvider localeProvider,
            ISignUpPhoneView view)
        {
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _accountService = accountService;
            _localeProvider = localeProvider;
            _view = view;
            signUpPhoneValidator = new SignUpPhoneValidator();
            signInPhoneValidator = new SignInPhoneValidator();
        }

        public void Init()
        {
            this.SetSelectedCountry(_localeProvider.GetCountryCode());
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
                    PhoneNumber = _view.CountryCodeField.Text + _view.PhoneNumberField.Text,
                    PhoneCountryCode = _view.CountryCodeField.Text.Substring(1),
                    CountryCode = this.selectedCountryCode
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

        public void CountryButtonClick()
        {
            _view.ShowCountryPicker(_localeProvider.GetCountryNames(), selectedCountryCode, SetSelectedCountry);
        }

        private void SetSelectedCountry(string countryCode)
        {
            this.selectedCountryCode = countryCode;
            SetPhoneCountryCode();
            SetCountryButton();
        }

        private void SetCountryButton()
        {
            _view.SelectCountryButton.TitleString = _localeProvider.GetCountryName(this.selectedCountryCode);
        }

        private void SetPhoneCountryCode()
        {
            string phoneCountryCode =
                PhoneCodes.CountryCodes.ContainsKey(selectedCountryCode)
                          ? PhoneCodes.CountryCodes[selectedCountryCode]
                          : PhoneCodes.CountryCodes["US"];
            this._view.CountryCodeField.Text = $"+{phoneCountryCode}";
        }
    }
}
