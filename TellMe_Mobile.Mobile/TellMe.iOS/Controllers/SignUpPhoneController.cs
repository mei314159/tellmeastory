using System;
using UIKit;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Local;
using System.Text.RegularExpressions;
using TellMe.iOS.Core.Providers;
using SharpMobileCode.ModalPicker;
using TellMe.iOS.Views.CustomPicker;
using System.Linq;
using System.Collections.Generic;

namespace TellMe.iOS
{
    public partial class SignUpPhoneController : UIViewController, ISignUpPhoneView
    {
        private SignupPhoneBusinessLogic _businessLogic;

        public SignUpPhoneController(IntPtr handle) : base(handle)
        {
        }

        public ITextInput PhoneNumberField => PhoneNumber;

        public ITextInput CountryCodeField => CountryCode;

        public IButton SelectCountryButton => CountryButton;

        public string ConfirmationCode => "0000";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = new SignupPhoneBusinessLogic(
                App.Instance.Router,
                new RemoteAccountDataService(),
                new AccountService(),
                new LocaleProvider(),
                this);
            this.CountryCode.ShouldChangeCharacters += CountryCode_ShouldChangeCharacters;
            this._businessLogic.Init();
        }

        async partial void ContinueButton_TouchUpInside(UIButton sender)
        {
            await _businessLogic.SignUpAsync();
        }

        public void ShowErrorMessage(string title, string message = null)
        {
            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController
                    .Create(title,
                            message ?? string.Empty,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                this.PresentViewController(alert, true, null);
            });
        }

        bool CountryCode_ShouldChangeCharacters(UITextField textField, Foundation.NSRange range, string replacementString)
        {
            string newText = textField.Text.Substring(0, (int)range.Location)
                                      + replacementString
                                      + textField.Text.Substring((int)(range.Location + range.Length));

            return Regex.IsMatch(newText, "^\\+\\d*$");
        }


		partial void CountryButton_TouchUpInside(Button sender)
		{
            _businessLogic.CountryButtonClick();
        }

        public async void ShowCountryPicker(IReadOnlyDictionary<string, string> countries, string selectedCountry, Action<string> callback)
        {
            var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Country", this)
            {
                //HeaderBackgroundColor = UIColor.White,
                HeaderTextColor = UIColor.DarkGray,
                TransitioningDelegate = new ModalPickerTransitionDelegate(),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };

            modalPicker.PickerType = ModalPickerType.Custom;
            nint selectedRow = 0;

            for (int i = 0; i < countries.Count; i++)
            {
                var item = countries.ElementAt(i);
                if (item.Key == selectedCountry)
                {
                    selectedRow = i;
                    break;
                }
            }

            modalPicker.PickerView.Model = new CustomPickerModel(countries.Select(a => a.Value).ToList());
            modalPicker.PickerView.Select(selectedRow, 0, false);

            modalPicker.OnModalPickerDismissed += (s, ea) =>
            {
                var index = modalPicker.PickerView.SelectedRowInComponent(0);
                var value = countries.ElementAt((int)index).Key;
                callback(value);
            };

            await this.PresentViewControllerAsync(modalPicker, true);
        }
    }
}