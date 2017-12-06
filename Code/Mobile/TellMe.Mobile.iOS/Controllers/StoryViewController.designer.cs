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

namespace TellMe.iOS.Controllers
{
    [Register ("StoryViewController")]
    partial class StoryViewController
    {
        [Outlet]
        UIKit.NSLayoutConstraint BottomOffset { get; set; }


        [Outlet]
        Button CancelButton { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint NewCommentHeight { get; set; }


        [Outlet]
        UIKit.UITextView NewCommentText { get; set; }


        [Outlet]
        UIKit.UILabel ReplyToLabel { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ReplyToWrapperHeight { get; set; }


        [Outlet]
        UIKit.UIButton SendButton { get; set; }


        [Outlet]
        UIKit.UITableView TableView { get; set; }


        [Action ("CancelButtonTouched:forEvent:")]
        partial void CancelButtonTouched (Button sender, UIKit.UIEvent @event);


        [Action ("SendButtonTouched:")]
        partial void SendButtonTouched (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (BottomOffset != null) {
                BottomOffset.Dispose ();
                BottomOffset = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (NewCommentHeight != null) {
                NewCommentHeight.Dispose ();
                NewCommentHeight = null;
            }

            if (NewCommentText != null) {
                NewCommentText.Dispose ();
                NewCommentText = null;
            }

            if (ReplyToLabel != null) {
                ReplyToLabel.Dispose ();
                ReplyToLabel = null;
            }

            if (ReplyToWrapperHeight != null) {
                ReplyToWrapperHeight.Dispose ();
                ReplyToWrapperHeight = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}