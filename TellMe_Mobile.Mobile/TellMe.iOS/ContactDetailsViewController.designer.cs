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
    [Register ("ContactDetailsViewController")]
    partial class ContactDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem NavItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RequestStoryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView StoriesTableView { get; set; }

        [Action ("RequestStoryButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RequestStoryButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (NavItem != null) {
                NavItem.Dispose ();
                NavItem = null;
            }

            if (RequestStoryButton != null) {
                RequestStoryButton.Dispose ();
                RequestStoryButton = null;
            }

            if (StoriesTableView != null) {
                StoriesTableView.Dispose ();
                StoriesTableView = null;
            }
        }
    }
}