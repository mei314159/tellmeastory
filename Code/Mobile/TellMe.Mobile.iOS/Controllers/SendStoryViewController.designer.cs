// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using TellMe.iOS.Core.UI;

namespace TellMe.iOS.Controllers
{
    [Register ("SendStoryViewController")]
    partial class SendStoryViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button ChooseRecipientsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button SendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StoryLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.TextInput StoryTitle { get; set; }

        [Action ("ChooseRecipientsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChooseRecipientsButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        [Action ("SendButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChooseRecipientsButton != null) {
                ChooseRecipientsButton.Dispose ();
                ChooseRecipientsButton = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }

            if (StoryLabel != null) {
                StoryLabel.Dispose ();
                StoryLabel = null;
            }

            if (StoryTitle != null) {
                StoryTitle.Dispose ();
                StoryTitle = null;
            }
        }
    }
}