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
    [Register ("EventViewController")]
    partial class EventViewController
    {
        [Outlet]
        UIKit.UINavigationItem NavItem { get; set; }


        [Outlet]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (NavItem != null) {
                NavItem.Dispose ();
                NavItem = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }
        }
    }
}