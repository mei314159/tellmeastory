using Foundation;
using System;
using UIKit;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.UI.Components;
using TellMe.iOS.Extensions;
using TellMe.Core.Services;
using TellMe.Core;

namespace TellMe.iOS
{
    public partial class SignInController : UIViewController, ISigninView
    {
        private AccountService accountService;

        public SignInController(IntPtr handle) : base(handle)
        {
        }

        public ITextInput EmailField => this.Email;

        public ITextInput PasswordField => this.Password;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.accountService = new AccountService(App.Instance.DataStorage);
        }

        async partial void SignInButton_TouchUpInside(UIButton sender)
        {
            var result = await accountService.SignInAsync(this);
            if (result.IsValid)
            {
                App.Instance.DataStorage.AuthInfo = result.Result.Data;
                this.View.Window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController());
                return;
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