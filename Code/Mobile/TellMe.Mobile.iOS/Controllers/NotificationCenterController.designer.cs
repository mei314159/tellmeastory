// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Controllers
{
    [Register ("NotificationCenterController")]
    partial class NotificationCenterController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        [Action ("CloseButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}