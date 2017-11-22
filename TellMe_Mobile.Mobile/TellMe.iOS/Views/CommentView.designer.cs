// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;
using TellMe.iOS.Core.UI;

namespace TellMe.iOS.Views
{
    [Register ("CommentView")]
    partial class CommentView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Button ReplyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label Text { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label Time { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label UserName { get; set; }

        [Action ("ReplyButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ReplyButton_TouchUpInside (Button sender);

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