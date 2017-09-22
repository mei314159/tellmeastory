using Foundation;
using System;
using UIKit;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Services;
using TellMe.Core;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class SignUpPhoneController : UIViewController, ISignUpPhoneView, ISigninPhoneView
    {
        private AccountService accountService;

        public SignUpPhoneController (IntPtr handle) : base (handle)
        {
        }

        public ITextInput PhoneNumberField => this.PhoneNumber;

        public string ConfirmationCode => "0000";

        string ISigninPhoneView.PhoneNumber => PhoneNumberField?.Text;

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.accountService = new AccountService(App.Instance.DataStorage);
		}


		async partial void ContinueButton_TouchUpInside(UIButton sender)
        {
			var result = await accountService.SignUpPhoneAsync(this);
			if (result.IsValid)
			{
                var authResult = await accountService.SignInPhoneAsync(this);
                if (authResult.IsValid)
				{
                    App.Instance.DataStorage.AuthInfo = authResult.Result.Data;
                    this.View.Window.SwapController(UIStoryboard.FromName("Auth", null).InstantiateViewController("ImportContactsController"));
					return;
				}

                result = authResult;
			}

			UIAlertController alert = UIAlertController
				.Create("Validation error",
						result.ErrorsString,
						UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (obj) =>
			{

			}));
			this.PresentViewController(alert, true, null);
        }
    }
}