// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TellMe.iOS.Controllers
{
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem EditButton { get; set; }


        [Action ("EditButton_Touched:")]
        partial void EditButton_Touched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (EditButton != null) {
                EditButton.Dispose ();
                EditButton = null;
            }
        }
    }
}