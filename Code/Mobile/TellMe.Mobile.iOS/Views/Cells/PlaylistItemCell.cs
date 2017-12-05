using System;
using Foundation;
using TellMe.Mobile.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class PlaylistItemCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("PlaylistItemCell");
        public static readonly UINib Nib;

        static PlaylistItemCell()
        {
            Nib = UINib.FromName("PlaylistItemCell", NSBundle.MainBundle);
        }

        protected PlaylistItemCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public PlaylistDTO Playlist { get; set; }
    }
}
