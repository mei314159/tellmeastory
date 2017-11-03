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
	[Register ("StorytellerViewController")]
	partial class StorytellerViewController
	{
		[Outlet]
		UIKit.UILabel FullName { get; set; }

		[Outlet]
		UIKit.UINavigationItem NavItem { get; set; }

		[Outlet]
		TellMe.iOS.Picture ProfilePicture { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView Spinner { get; set; }

		[Outlet]
		UIKit.UILabel UserName { get; set; }

		[Action ("RequestStoryTouched:")]
		partial void RequestStoryTouched (Foundation.NSObject sender);

		[Action ("SendStoryTouched:")]
		partial void SendStoryTouched (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FullName != null) {
				FullName.Dispose ();
				FullName = null;
			}

			if (ProfilePicture != null) {
				ProfilePicture.Dispose ();
				ProfilePicture = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}

			if (UserName != null) {
				UserName.Dispose ();
				UserName = null;
			}

			if (NavItem != null) {
				NavItem.Dispose ();
				NavItem = null;
			}
		}
	}
}
