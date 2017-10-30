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
    [Register ("TribesListCell")]
    partial class TribesListCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Label MembershipStatus { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Picture ProfilePicture { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MembershipStatus != null) {
                MembershipStatus.Dispose ();
                MembershipStatus = null;
            }

            if (Name != null) {
                Name.Dispose ();
                Name = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }
        }
    }
}