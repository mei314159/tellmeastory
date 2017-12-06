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
	[Register ("SlimStoryCell")]
	partial class SlimStoryCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Core.UI.Picture Preview { get; set; }

		[Outlet]
		TellMe.iOS.Core.UI.Picture SenderPicture { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Preview != null) {
				Preview.Dispose ();
				Preview = null;
			}

			if (SenderPicture != null) {
				SenderPicture.Dispose ();
				SenderPicture = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}
		}
	}
}
