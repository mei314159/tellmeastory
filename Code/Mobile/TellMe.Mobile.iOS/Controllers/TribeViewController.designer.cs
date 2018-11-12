// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Controllers
{
	[Register ("TribeViewController")]
	partial class TribeViewController
	{
		[Outlet]
		UIKit.UILabel EventsCount { get; set; }

		[Outlet]
		UIKit.UIView HeaderView { get; set; }

		[Outlet]
		UIKit.UILabel MembersCount { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView Spinner { get; set; }

		[Outlet]
		UIKit.UILabel StoriesCount { get; set; }

		[Outlet]
		UIKit.UILabel TribeName { get; set; }

		[Action ("BackButtonTouched:")]
		partial void BackButtonTouched (Foundation.NSObject sender);

		[Action ("InfoButtonTouched:")]
		partial void InfoButtonTouched (Foundation.NSObject sender);

		[Action ("RequestStoryTouched:")]
		partial void RequestStoryTouched (Foundation.NSObject sender);

		[Action ("SendStoryTouched:")]
		partial void SendStoryTouched (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (EventsCount != null) {
				EventsCount.Dispose ();
				EventsCount = null;
			}

			if (MembersCount != null) {
				MembersCount.Dispose ();
				MembersCount = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}

			if (StoriesCount != null) {
				StoriesCount.Dispose ();
				StoriesCount = null;
			}

			if (TribeName != null) {
				TribeName.Dispose ();
				TribeName = null;
			}

			if (HeaderView != null) {
				HeaderView.Dispose ();
				HeaderView = null;
			}
		}
	}
}
