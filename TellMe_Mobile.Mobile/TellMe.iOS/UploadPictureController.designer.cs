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
    [Register ("UploadPictureController")]
    partial class UploadPictureController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContinueButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Picture Picture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SkipButton { get; set; }

        [Action ("ContinueButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ContinueButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SkipButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SkipButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContinueButton != null) {
                ContinueButton.Dispose ();
                ContinueButton = null;
            }

            if (Picture != null) {
                Picture.Dispose ();
                Picture = null;
            }

            if (SkipButton != null) {
                SkipButton.Dispose ();
                SkipButton = null;
            }
        }
    }
}