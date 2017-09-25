using System;
using UIKit;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core;
using TellMe.Core.Types.BusinessLogic;

namespace TellMe.iOS
{
    public partial class SignUpPhoneController : UIViewController, ISignUpPhoneView
    {
        private SignupPhoneBusinessLogic _businessLogic;

        public SignUpPhoneController(IntPtr handle) : base(handle)
        {
        }

        public ITextInput PhoneNumberField => PhoneNumber;

        public string ConfirmationCode => "0000";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = new SignupPhoneBusinessLogic(App.Instance.Router, new AccountService(App.Instance.DataStorage), this);
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

    }
}