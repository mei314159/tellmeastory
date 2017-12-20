// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using System.CodeDom.Compiler;
using Foundation;

namespace TellMe.iOS.Views
{
	[Register ("AccountPictureView")]
	partial class AccountPictureView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView PictureTouchWrapper { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PictureTouchWrapper != null) {
				PictureTouchWrapper.Dispose ();
				PictureTouchWrapper = null;
			}

			if (ProfilePicture != null) {
				ProfilePicture.Dispose ();
				ProfilePicture = null;
			}
		}
	}
}
