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
	[Register ("TribeViewController")]
	partial class TribeViewController
	{
		[Outlet]
		UIKit.UINavigationItem NavItem { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView Spinner { get; set; }

		[Outlet]
		UIKit.UILabel TribeName { get; set; }

		[Action ("InfoButtonTouched:")]
		partial void InfoButtonTouched (Foundation.NSObject sender);

		[Action ("RequestStoryTouched:")]
		partial void RequestStoryTouched (Foundation.NSObject sender);

		[Action ("SendStoryTouched:")]
		partial void SendStoryTouched (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (NavItem != null) {
				NavItem.Dispose ();
				NavItem = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}

			if (TribeName != null) {
				TribeName.Dispose ();
				TribeName = null;
			}
		}
	}
}
