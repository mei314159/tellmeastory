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
    [Register ("StoriesListCell")]
    partial class StoriesListCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Button CommentsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Button LikeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Picture Preview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView ReceiversCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label Title { get; set; }

        [Action ("CommentsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CommentsButton_TouchUpInside (Button sender);

        [Action ("LikeButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LikeButton_TouchUpInside (Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (CommentsButton != null) {
                CommentsButton.Dispose ();
                CommentsButton = null;
            }

            if (LikeButton != null) {
                LikeButton.Dispose ();
                LikeButton = null;
            }

            if (Preview != null) {
                Preview.Dispose ();
                Preview = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (ReceiversCollection != null) {
                ReceiversCollection.Dispose ();
                ReceiversCollection = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}