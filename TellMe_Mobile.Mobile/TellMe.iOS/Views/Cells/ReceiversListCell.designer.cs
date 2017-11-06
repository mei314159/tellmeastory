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
	[Register ("ReceiversListCell")]
	partial class ReceiversListCell
	{
		[Outlet]
		TellMe.iOS.Picture ProfilePicture { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProfilePicture != null) {
				ProfilePicture.Dispose ();
				ProfilePicture = null;
			}
		}
	}
}
