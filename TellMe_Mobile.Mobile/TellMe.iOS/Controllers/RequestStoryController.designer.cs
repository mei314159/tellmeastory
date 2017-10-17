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
    [Register ("RequestStoryController")]
    partial class RequestStoryController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RequestTextPreview { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button SendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput StoryTitle { get; set; }

        [Action ("SendButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButton_TouchUpInside (TellMe.iOS.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (RequestTextPreview != null) {
                RequestTextPreview.Dispose ();
                RequestTextPreview = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }

            if (StoryTitle != null) {
                StoryTitle.Dispose ();
                StoryTitle = null;
            }
        }
    }
}