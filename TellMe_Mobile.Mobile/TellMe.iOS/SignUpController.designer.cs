// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TellMe.iOS
{
    [Register ("SignUpController")]
    partial class SignUpController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput ConfirmPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput Email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SignUpButton { get; set; }

        [Action ("ButtonTouchd:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonTouchd (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ConfirmPassword != null) {
                ConfirmPassword.Dispose ();
                ConfirmPassword = null;
            }

            if (Email != null) {
                Email.Dispose ();
                Email = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (SignUpButton != null) {
                SignUpButton.Dispose ();
                SignUpButton = null;
            }
        }
    }
}