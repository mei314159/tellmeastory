// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Views
{
    [Register ("AccountPictureView")]
    partial class AccountPictureView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PictureTouchWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PictureTouchWrapper != null) {
                PictureTouchWrapper.Dispose ();
                PictureTouchWrapper = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }
        }
    }
}