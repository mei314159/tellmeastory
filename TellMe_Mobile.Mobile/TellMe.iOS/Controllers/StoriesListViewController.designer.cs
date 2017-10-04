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
    [Register ("StoriesListViewController")]
    partial class StoriesListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem RequestStoryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SendStoryButton { get; set; }

        [Action ("RequestStoryButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RequestStoryButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SendStoryButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendStoryButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (RequestStoryButton != null) {
                RequestStoryButton.Dispose ();
                RequestStoryButton = null;
            }

            if (SendStoryButton != null) {
                SendStoryButton.Dispose ();
                SendStoryButton = null;
            }
        }
    }
}