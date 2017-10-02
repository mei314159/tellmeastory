using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignUpPhoneView : IView
    {
        ITextInput CountryCodeField { get; }

        ITextInput PhoneNumberField { get; }

        IButton SelectCountryButton { get; }

        string ConfirmationCode { get; }

        void ShowCountryPicker(IReadOnlyDictionary<string, string> countries, string selectedCountry, Action<string> callback);
    }
}