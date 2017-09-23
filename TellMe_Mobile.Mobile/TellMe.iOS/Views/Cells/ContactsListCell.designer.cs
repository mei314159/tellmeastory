// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Views.Cells
{
    [Register ("ContactsListCell")]
    partial class ContactsListCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton InviteButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PhoneNumber { get; set; }

        [Action ("InviteButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void InviteButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (InviteButton != null) {
                InviteButton.Dispose ();
                InviteButton = null;
            }

            if (Name != null) {
                Name.Dispose ();
                Name = null;
            }

            if (PhoneNumber != null) {
                PhoneNumber.Dispose ();
                PhoneNumber = null;
            }
        }
    }
}