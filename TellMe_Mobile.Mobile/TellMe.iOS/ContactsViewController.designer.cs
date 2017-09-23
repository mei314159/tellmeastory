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
    [Register ("ContactsViewController")]
    partial class ContactsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ImportButton { get; set; }

        [Action ("ImportButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ImportButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ImportButton != null) {
                ImportButton.Dispose ();
                ImportButton = null;
            }
        }
    }
}