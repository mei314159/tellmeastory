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

namespace TellMe.iOS.Views.Cells
{
    [Register ("TribeMembersListCell")]
    partial class TribeMembersListCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FullName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Username { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FullName != null) {
                FullName.Dispose ();
                FullName = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (StatusLabel != null) {
                StatusLabel.Dispose ();
                StatusLabel = null;
            }

            if (Username != null) {
                Username.Dispose ();
                Username = null;
            }
        }
    }
}