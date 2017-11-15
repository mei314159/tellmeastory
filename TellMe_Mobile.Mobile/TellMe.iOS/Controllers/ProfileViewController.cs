using System;
using UIKit;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.iOS.Extensions;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;

namespace TellMe.iOS
{
    public partial class ProfileViewController : UIViewController, IView
    {
        private IRouter _router;
        private ILocalAccountService _localAccountService;

        public ProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            _router = IoC.Container.GetInstance<IRouter>();
            _localAccountService = IoC.Container.GetInstance<ILocalAccountService>();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, true);
        }

        partial void SignOutButton_TouchUpInside(UIButton sender)
        {
            _localAccountService.SaveAuthInfo(null);
            _router.SwapToAuth();
        }

        partial void ChangePictureButton_TouchUpInside(UIButton sender)
        {
            _router.NavigateSetProfilePicture(this);
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);
    }
}