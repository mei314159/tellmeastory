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
    [Register ("ReceiversListCell")]
    partial class ReceiversListCell
    {
        [Outlet]
        Picture ProfilePicture { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }
        }
    }
}