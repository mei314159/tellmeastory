// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;
using TellMe.iOS.Core.UI;

namespace TellMe.iOS.Views.Cells
{
    [Register ("StoryViewCell")]
    partial class StoryViewCell
    {
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
        Button ReplayButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label Title { get; set; }

        [Action ("LikeButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LikeButton_TouchUpInside (Button sender);

        [Action ("ReplayButton_Touched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ReplayButton_Touched (Button sender);

        void ReleaseDesignerOutlets ()
        {
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

            if (ReplayButton != null) {
                ReplayButton.Dispose ();
                ReplayButton = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}