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
    [Register ("ImportContactsController")]
    partial class ImportContactsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ProvideAccessButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SkipButton { get; set; }

        [Action ("ProvideAccessButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ProvideAccessButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ProvideAccessButton != null) {
                ProvideAccessButton.Dispose ();
                ProvideAccessButton = null;
            }

            if (SkipButton != null) {
                SkipButton.Dispose ();
                SkipButton = null;
            }
        }
    }
}