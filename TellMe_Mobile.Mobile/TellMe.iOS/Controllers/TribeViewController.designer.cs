// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using Foundation;

namespace TellMe.iOS.Controllers
{
    [Register ("TribeViewController")]
    partial class TribeViewController
    {
        [Outlet]
        UIKit.UINavigationItem NavItem { get; set; }


        [Outlet]
        UIKit.UIActivityIndicatorView Spinner { get; set; }


        [Outlet]
        UIKit.UILabel TribeName { get; set; }


        [Action ("InfoButtonTouched:")]
        partial void InfoButtonTouched (Foundation.NSObject sender);


        [Action ("RequestStoryTouched:")]
        partial void RequestStoryTouched (Foundation.NSObject sender);


        [Action ("SendStoryTouched:")]
        partial void SendStoryTouched (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (NavItem != null) {
                NavItem.Dispose ();
                NavItem = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (TribeName != null) {
                TribeName.Dispose ();
                TribeName = null;
            }
        }
    }
}