// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using TellMe.iOS.Core.UI;

namespace TellMe.iOS.Views.Cells
{
    [Register ("CommentViewCell")]
    partial class CommentViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView Replies { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button ReplyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button ShowRepliesButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label Text { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label Time { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label UserName { get; set; }

        [Action ("ReplyButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ReplyButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        [Action ("ShowRepliesButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowRepliesButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (Replies != null) {
                Replies.Dispose ();
                Replies = null;
            }

            if (ReplyButton != null) {
                ReplyButton.Dispose ();
                ReplyButton = null;
            }

            if (ShowRepliesButton != null) {
                ShowRepliesButton.Dispose ();
                ShowRepliesButton = null;
            }

            if (Text != null) {
                Text.Dispose ();
                Text = null;
            }

            if (Time != null) {
                Time.Dispose ();
                Time = null;
            }

            if (UserName != null) {
                UserName.Dispose ();
                UserName = null;
            }
        }
    }
}