// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;
using TellMe.iOS.Core.UI;

namespace TellMe.iOS.Controllers
{
    [Register ("RecordVideoController")]
    partial class RecordVideoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Label Duration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PreviewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Button RecordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SwitchCameraButton { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("RecordButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RecordButtonTouched (Button sender);

        [Action ("SwitchCameraButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SwitchCameraButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (Duration != null) {
                Duration.Dispose ();
                Duration = null;
            }

            if (PreviewView != null) {
                PreviewView.Dispose ();
                PreviewView = null;
            }

            if (RecordButton != null) {
                RecordButton.Dispose ();
                RecordButton = null;
            }

            if (SwitchCameraButton != null) {
                SwitchCameraButton.Dispose ();
                SwitchCameraButton = null;
            }
        }
    }
}