// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TellMe.iOS
{
    [Register ("PreviewVideoController")]
    partial class PreviewVideoController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PreviewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button SendButton { get; set; }

        [Action ("SendButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButtonTouched (TellMe.iOS.Button sender);

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