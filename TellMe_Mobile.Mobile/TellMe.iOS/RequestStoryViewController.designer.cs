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
    [Register ("RequestStoryViewController")]
    partial class RequestStoryViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button ChooseRecipientsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView RecipientsTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Button SendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.TextInput StoryName { get; set; }

        [Action ("ChooseRecipientsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChooseRecipientsButton_TouchUpInside (TellMe.iOS.Button sender);

        [Action ("CloseButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("SendButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButton_TouchUpInside (TellMe.iOS.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChooseRecipientsButton != null) {
                ChooseRecipientsButton.Dispose ();
                ChooseRecipientsButton = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (RecipientsTable != null) {
                RecipientsTable.Dispose ();
                RecipientsTable = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }

            if (StoryName != null) {
                StoryName.Dispose ();
                StoryName = null;
            }
        }
    }
}