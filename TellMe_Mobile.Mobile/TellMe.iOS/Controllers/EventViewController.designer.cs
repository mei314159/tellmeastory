// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TellMe.iOS
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
            if (NavItem != null)
            {
                NavItem.Dispose();
                NavItem = null;
            }

            if (Spinner != null)
            {
                Spinner.Dispose();
                Spinner = null;
            }
		}
	}
}
