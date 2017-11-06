// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS
{
    [Register ("StoryViewController")]
    partial class StoryViewController
    {
        [Outlet]
        UIKit.UINavigationItem NavItem { get; set; }


        [Outlet]
        UIKit.UIScrollView ScrollView { get; set; }


        [Outlet]
        UIKit.UIView StoryViewWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Picture Preview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Label Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Preview != null) {
                Preview.Dispose ();
                Preview = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (StoryViewWrapper != null) {
                StoryViewWrapper.Dispose ();
                StoryViewWrapper = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}