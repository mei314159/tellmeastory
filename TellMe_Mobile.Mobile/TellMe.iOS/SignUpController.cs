using System;
using UIKit;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Services;
using TellMe.Core;
using TellMe.iOS.Extensions;
using System.Linq;

namespace TellMe.iOS
{
    public partial class SignUpController : UIViewController, ISignupView
    {
        private AccountService accountService;

        public SignUpController(IntPtr handle) : base(handle)
        {
        }

        public ITextInput EmailField => this.Email;

        public ITextInput PasswordField => this.Password;

        public ITextInput ConfirmPasswordField => this.ConfirmPassword;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.accountService = new AccountService(App.Instance.DataStorage);
        }

        async partial void ButtonTouchd(UIButton sender)
        {
            var result = await accountService.SignUpAsync(this);
            if (result.IsValid)
            {
				var authResult = await accountService.SignInAsync(this);
				if (authResult.IsValid)
                {
                    App.Instance.DataStorage.AuthInfo = authResult.Result.Data;
                    this.View.Window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController());
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