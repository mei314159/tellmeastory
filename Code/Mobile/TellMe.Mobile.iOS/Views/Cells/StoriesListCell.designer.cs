// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Views.Cells
{
	[Register ("StoriesListCell")]
	partial class StoriesListCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Core.UI.Button CommentsButton { get; set; }

		[Outlet]
		UIKit.UIView ContentWrapper { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Core.UI.Button LikeButton { get; set; }

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
			if (ContentWrapper != null) {
				ContentWrapper.Dispose ();
				ContentWrapper = null;
			}

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
