// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Views.Cells
{
    [Register ("StoriesListCell")]
    partial class StoriesListCell
    {
        [Outlet]
        UIKit.UIView ContentWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button CommentsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button LikeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MoreButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label ObjectionableLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture Preview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView ReceiversCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label Title { get; set; }


        [Action ("CommentsButton_TouchUpInside:")]
        partial void CommentsButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);


        [Action ("LikeButton_TouchUpInside:")]
        partial void LikeButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);


        [Action ("MoreButton_Touched:")]
        partial void MoreButton_Touched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CommentsButton != null) {
                CommentsButton.Dispose ();
                CommentsButton = null;
            }

            if (ContentWrapper != null) {
                ContentWrapper.Dispose ();
                ContentWrapper = null;
            }

            if (LikeButton != null) {
                LikeButton.Dispose ();
                LikeButton = null;
            }

            if (MoreButton != null) {
                MoreButton.Dispose ();
                MoreButton = null;
            }

            if (ObjectionableLabel != null) {
                ObjectionableLabel.Dispose ();
                ObjectionableLabel = null;
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