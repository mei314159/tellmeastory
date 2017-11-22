// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using System.CodeDom.Compiler;
using Foundation;

namespace TellMe.iOS.Controllers
{
	[Register ("StoriesListViewController")]
	partial class StoriesListViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem AccountSettingsButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem Events { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem Notifications { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIBarButtonItem Storytellers { get; set; }

		[Action ("AccountSettingsButton_Activated:")]
		partial void AccountSettingsButton_Activated (UIKit.UIBarButtonItem sender);

		[Action ("Events_Activated:")]
		partial void Events_Activated (UIKit.UIBarButtonItem sender);

		[Action ("Notifications_Activated:")]
		partial void Notifications_Activated (UIKit.UIBarButtonItem sender);

		[Action ("Storytellers_Activated:")]
		partial void Storytellers_Activated (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AccountSettingsButton != null) {
				AccountSettingsButton.Dispose ();
				AccountSettingsButton = null;
			}

			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (Notifications != null) {
				Notifications.Dispose ();
				Notifications = null;
			}

			if (Storytellers != null) {
				Storytellers.Dispose ();
				Storytellers = null;
			}

			if (Events != null) {
				Events.Dispose ();
				Events = null;
			}
		}
	}
}
