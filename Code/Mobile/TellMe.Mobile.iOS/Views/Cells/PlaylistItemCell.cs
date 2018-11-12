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
        private PlaylistDTO _playlist;
        private static readonly UIImage DefaultPicture;

        static PlaylistItemCell()
        {
            Nib = UINib.FromName("PlaylistItemCell", NSBundle.MainBundle);
            DefaultPicture = UIImage.FromBundle("UserPic");
        }

        protected PlaylistItemCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public PlaylistDTO Playlist {
            get => _playlist;
            set
            {
                _playlist = value;
                this.Initialize();
            }
        }
        
        private void Initialize()
        {
            this.Title.Text = Playlist.Name;
            this.Count.Text = Playlist.StoriesCount + " Stories";
        }
    }
}
