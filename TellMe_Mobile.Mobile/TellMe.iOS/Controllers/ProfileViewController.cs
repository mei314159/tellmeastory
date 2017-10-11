using Foundation;
using System;
using UIKit;
using TellMe.Core;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class ProfileViewController : UIViewController
    {
        public ProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, true);
        }

        partial void SignOutButton_TouchUpInside(UIButton sender)
        {
            App.Instance.AuthInfo = null;
            var initialController = UIStoryboard
                .FromName("Auth", null).InstantiateInitialViewController();
            View.Window.SwapController(initialController);
        }
    }
}