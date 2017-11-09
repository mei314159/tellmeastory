// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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

			if (BottomOffset != null) {
				BottomOffset.Dispose ();
				BottomOffset = null;
			}
		}
	}
}
