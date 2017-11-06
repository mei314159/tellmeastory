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
		UIKit.UINavigationItem NavItem { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Picture Preview { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Picture ProfilePicture { get; set; }

		[Outlet]
		UIKit.UICollectionView ReceiversCollection { get; set; }

		[Outlet]
		UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIActivityIndicatorView Spinner { get; set; }

		[Outlet]
		UIKit.UIView StoryViewWrapper { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Label Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NavItem != null) {
				NavItem.Dispose ();
				NavItem = null;
			}

			if (Preview != null) {
				Preview.Dispose ();
				Preview = null;
			}

			if (ProfilePicture != null) {
				ProfilePicture.Dispose ();
				ProfilePicture = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}

			if (StoryViewWrapper != null) {
				StoryViewWrapper.Dispose ();
				StoryViewWrapper = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (ReceiversCollection != null) {
				ReceiversCollection.Dispose ();
				ReceiversCollection = null;
			}
		}
	}
}
