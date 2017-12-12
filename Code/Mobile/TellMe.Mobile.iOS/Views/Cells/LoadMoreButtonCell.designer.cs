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
    [Register ("LoadMoreButtonCell")]
    partial class LoadMoreButtonCell
    {
        [Outlet]
        UIKit.UIActivityIndicatorView Spinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button Button { get; set; }


        [Action ("Button_TouchUpInside:")]
        partial void Button_TouchUpInside (Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (Button != null) {
                Button.Dispose ();
                Button = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }
        }
    }
}