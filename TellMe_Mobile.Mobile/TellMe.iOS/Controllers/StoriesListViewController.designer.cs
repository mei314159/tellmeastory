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

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem Notifications { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem Storytellers { get; set; }

        [Action ("AccountSettingsButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AccountSettingsButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("Notifications_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Notifications_Activated (UIKit.UIBarButtonItem sender);

        [Action ("Storytellers_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Storytellers_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (AccountSettingsButton != null) {
                AccountSettingsButton.Dispose ();
                AccountSettingsButton = null;
            }

            if (ActivityIndicator != null) {
                ActivityIndicator.Dispose ();
                ActivityIndicator = null;
            }

            if (Notifications != null) {
                Notifications.Dispose ();
                Notifications = null;
            }

            if (Storytellers != null) {
                Storytellers.Dispose ();
                Storytellers = null;
            }
        }
    }
}