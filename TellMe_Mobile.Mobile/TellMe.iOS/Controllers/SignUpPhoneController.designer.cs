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
    [Register ("SignUpPhoneController")]
    partial class SignUpPhoneController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContinueButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button CountryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput CountryCode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput PhoneNumber { get; set; }

        [Action ("ContinueButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ContinueButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("CountryButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CountryButton_TouchUpInside (TellMe.iOS.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContinueButton != null) {
                ContinueButton.Dispose ();
                ContinueButton = null;
            }

            if (CountryButton != null) {
                CountryButton.Dispose ();
                CountryButton = null;
            }

            if (CountryCode != null) {
                CountryCode.Dispose ();
                CountryCode = null;
            }

            if (PhoneNumber != null) {
                PhoneNumber.Dispose ();
                PhoneNumber = null;
            }
        }
    }
}