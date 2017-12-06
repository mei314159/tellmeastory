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
    [Register ("EventCell")]
    partial class EventCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button AcceptButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView AttendeesCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label DateDay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label DateMonth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label Description { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Picture ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Button SkipButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TellMe.iOS.Core.UI.Label Title { get; set; }

        [Action ("AcceptButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AcceptButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        [Action ("SkipButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SkipButton_TouchUpInside (TellMe.iOS.Core.UI.Button sender);

        void ReleaseDesignerOutlets ()
        {
            if (AcceptButton != null) {
                AcceptButton.Dispose ();
                AcceptButton = null;
            }

            if (AttendeesCollection != null) {
                AttendeesCollection.Dispose ();
                AttendeesCollection = null;
            }

            if (ContentWrapper != null) {
                ContentWrapper.Dispose ();
                ContentWrapper = null;
            }

            if (DateDay != null) {
                DateDay.Dispose ();
                DateDay = null;
            }

            if (DateMonth != null) {
                DateMonth.Dispose ();
                DateMonth = null;
            }

            if (Description != null) {
                Description.Dispose ();
                Description = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SkipButton != null) {
                SkipButton.Dispose ();
                SkipButton = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}