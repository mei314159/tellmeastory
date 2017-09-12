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
    [Register ("SignInController")]
    partial class SignInController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput Email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SignInButton { get; set; }

        [Action ("SignInButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignInButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Email != null) {
                Email.Dispose ();
                Email = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (SignInButton != null) {
                SignInButton.Dispose ();
                SignInButton = null;
            }
        }
    }
}