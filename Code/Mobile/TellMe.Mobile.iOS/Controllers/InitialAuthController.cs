using System;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class InitialAuthController : UIViewController
    {
        public InitialAuthController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewDidDisappear(bool animated)
        {
            NavigationController.SetNavigationBarHidden(false, true);
        }
    }
}