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
    [Register ("PreviewVideoController")]
    partial class PreviewVideoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PreviewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Button SendButton { get; set; }

        [Action ("SendButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButtonTouched (Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (PreviewView != null) {
                PreviewView.Dispose ();
                PreviewView = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }
        }
    }
}