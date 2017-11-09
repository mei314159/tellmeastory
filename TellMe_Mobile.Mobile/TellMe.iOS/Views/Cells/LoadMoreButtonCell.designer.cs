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
	[Register ("LoadMoreButtonCell")]
	partial class LoadMoreButtonCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		TellMe.iOS.Button Button { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView Spinner { get; set; }

		[Action ("Button_TouchUpInside:")]
		partial void Button_TouchUpInside (TellMe.iOS.Button sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Button != null) {
				Button.Dispose ();
				Button = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}
		}
	}
}
