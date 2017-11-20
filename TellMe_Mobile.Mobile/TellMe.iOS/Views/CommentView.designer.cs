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
    [Register ("CommentView")]
    partial class CommentView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button ReplyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Label Text { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Label Time { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Label UserName { get; set; }

        [Action ("ReplyButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ReplyButton_TouchUpInside (TellMe.iOS.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (ReplyButton != null) {
                ReplyButton.Dispose ();
                ReplyButton = null;
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