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
    [Register ("StoriesListViewController")]
    partial class StoriesListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem AccountSettingsButton { get; set; }

        [Action ("AccountSettingsButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AccountSettingsButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (AccountSettingsButton != null) {
                AccountSettingsButton.Dispose ();
                AccountSettingsButton = null;
            }
        }
    }
}