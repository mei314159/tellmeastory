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
    [Register ("StorytellerViewController")]
    partial class StorytellerViewController
    {
        [Outlet]
        UIKit.UILabel FullName { get; set; }


        [Outlet]
        UIKit.UINavigationItem NavItem { get; set; }


        [Outlet]
        TellMe.iOS.Picture ProfilePicture { get; set; }


        [Outlet]
        UIKit.UIActivityIndicatorView Spinner { get; set; }


        [Outlet]
        UIKit.UILabel UserName { get; set; }


        [Action ("RequestStoryTouched:")]
        partial void RequestStoryTouched (Foundation.NSObject sender);


        [Action ("SendStoryTouched:")]
        partial void SendStoryTouched (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (FullName != null) {
                FullName.Dispose ();
                FullName = null;
            }

            if (NavItem != null) {
                NavItem.Dispose ();
                NavItem = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (UserName != null) {
                UserName.Dispose ();
                UserName = null;
            }
        }
    }
}