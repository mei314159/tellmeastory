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
    [Register ("PlaylistItemCell")]
    partial class PlaylistItemCell
    {
        [Outlet]
        UIKit.UILabel Count { get; set; }


        [Outlet]
        Picture Picture { get; set; }


        [Outlet]
        UIKit.UILabel Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Count != null) {
                Count.Dispose ();
                Count = null;
            }

            if (Picture != null) {
                Picture.Dispose ();
                Picture = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}