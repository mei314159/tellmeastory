// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using Foundation;

namespace TellMe.iOS.Views.Cells
{
	[Register ("PlaylistItemCell")]
	partial class PlaylistItemCell
	{
		[Outlet]
		UIKit.UILabel Count { get; set; }

		[Outlet]
		UIKit.UIImageView Picture { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Picture != null) {
				Picture.Dispose ();
				Picture = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (Count != null) {
				Count.Dispose ();
				Count = null;
			}
		}
	}
}
