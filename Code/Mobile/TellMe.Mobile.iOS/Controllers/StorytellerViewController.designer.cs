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
    [Register ("StorytellerViewController")]
    partial class StorytellerViewController
    {
        [Outlet]
        UIKit.UIButton BackButton { get; set; }


        [Outlet]
        UIKit.UILabel EventsCount { get; set; }


        [Outlet]
        UIKit.UILabel FriendsCount { get; set; }


        [Outlet]
        UIKit.UILabel FullName { get; set; }


        [Outlet]
        UIKit.UINavigationItem NavItem { get; set; }


        [Outlet]
        TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }


        [Outlet]
        UIKit.UIActivityIndicatorView Spinner { get; set; }


        [Outlet]
        UIKit.UILabel StoriesCount { get; set; }


        [Outlet]
        UIKit.UILabel UserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView HeaderView { get; set; }


        [Action ("BackButtonTouched:forEvent:")]
        partial void BackButtonTouched (UIKit.UIButton sender, UIKit.UIEvent @event);


        [Action ("RequestStoryTouched:")]
        partial void RequestStoryTouched (Foundation.NSObject sender);


        [Action ("SendStoryTouched:")]
        partial void SendStoryTouched (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackButton != null) {
                BackButton.Dispose ();
                BackButton = null;
            }

            if (EventsCount != null) {
                EventsCount.Dispose ();
                EventsCount = null;
            }

            if (FriendsCount != null) {
                FriendsCount.Dispose ();
                FriendsCount = null;
            }

            if (FullName != null) {
                FullName.Dispose ();
                FullName = null;
            }

            if (HeaderView != null) {
                HeaderView.Dispose ();
                HeaderView = null;
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

            if (StoriesCount != null) {
                StoriesCount.Dispose ();
                StoriesCount = null;
            }

            if (UserName != null) {
                UserName.Dispose ();
                UserName = null;
            }
        }
    }
}