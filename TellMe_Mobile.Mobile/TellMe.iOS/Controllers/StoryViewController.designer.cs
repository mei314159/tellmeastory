// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS
{
    [Register ("StoryViewController")]
    partial class StoryViewController
    {
        [Outlet]
        UIKit.NSLayoutConstraint BottomOffset { get; set; }


        [Outlet]
        UIKit.UITextView NewCommentText { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint NewCommentWrapperHeight { get; set; }


        [Outlet]
        UIKit.UIButton SendButton { get; set; }


        [Outlet]
        UIKit.UITableView TableView { get; set; }


        [Action ("SendButtonTouched:")]
        partial void SendButtonTouched (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (BottomOffset != null) {
                BottomOffset.Dispose ();
                BottomOffset = null;
            }

            if (NewCommentText != null) {
                NewCommentText.Dispose ();
                NewCommentText = null;
            }

            if (NewCommentWrapperHeight != null) {
                NewCommentWrapperHeight.Dispose ();
                NewCommentWrapperHeight = null;
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