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
    [Register ("StoriesListCell")]
    partial class StoriesListCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.StoryView StoryView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (StoryView != null) {
                StoryView.Dispose ();
                StoryView = null;
            }
        }
    }
}