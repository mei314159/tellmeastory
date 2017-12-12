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
    [Register ("PlaylistViewController")]
    partial class PlaylistViewController
    {
        [Outlet]
        UIKit.UINavigationBar NavigationBar { get; set; }


        [Outlet]
        UIKit.UINavigationItem NavigationItem { get; set; }


        [Outlet]
        UIKit.UITableView TableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PlayerWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture Preview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PreviousButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        [Action ("CloseButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("NextButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void NextButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("PreviousButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PreviousButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (NavigationBar != null) {
                NavigationBar.Dispose ();
                NavigationBar = null;
            }

            if (NavigationItem != null) {
                NavigationItem.Dispose ();
                NavigationItem = null;
            }

            if (NextButton != null) {
                NextButton.Dispose ();
                NextButton = null;
            }

            if (PlayerWrapper != null) {
                PlayerWrapper.Dispose ();
                PlayerWrapper = null;
            }

            if (Preview != null) {
                Preview.Dispose ();
                Preview = null;
            }

            if (PreviousButton != null) {
                PreviousButton.Dispose ();
                PreviousButton = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }

            if (View != null) {
                View.Dispose ();
                View = null;
            }
        }
    }
}