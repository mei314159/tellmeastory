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
    [Register ("NotificationCenterStoryCell")]
    partial class NotificationCenterStoryCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PictureView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture StoryPreview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Text { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PictureView != null) {
                PictureView.Dispose ();
                PictureView = null;
            }

            if (StoryPreview != null) {
                StoryPreview.Dispose ();
                StoryPreview = null;
            }

            if (Text != null) {
                Text.Dispose ();
                Text = null;
            }
        }
    }
}