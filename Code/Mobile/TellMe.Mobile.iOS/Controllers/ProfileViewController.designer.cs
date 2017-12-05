// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;

namespace TellMe.iOS.Controllers
{
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangePictureButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SignOutButton { get; set; }

        [Action ("ChangePictureButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangePictureButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SignOutButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignOutButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChangePictureButton != null) {
                ChangePictureButton.Dispose ();
                ChangePictureButton = null;
            }

            if (SignOutButton != null) {
                SignOutButton.Dispose ();
                SignOutButton = null;
            }
        }
    }
}