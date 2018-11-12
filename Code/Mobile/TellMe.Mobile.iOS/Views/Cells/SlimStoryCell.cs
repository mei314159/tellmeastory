using System;
using Foundation;
using TellMe.Shared.Contracts.DTO.Interfaces;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class SlimStoryCell : UITableViewCell
    {
        private IStoryDTO _story;

        public static readonly NSString Key = new NSString("SlimStoryCell");
        public static readonly UINib Nib;
        private static readonly UIImage DefaultUserPicture;
        private static readonly UIImage DefaultPlaylistPicture;

        static SlimStoryCell()
        {
            Nib = UINib.FromName("SlimStoryCell", NSBundle.MainBundle);
            DefaultUserPicture = UIImage.FromBundle("UserPic");
            DefaultPlaylistPicture = UIImage.FromBundle("Playlist");
        }

        protected SlimStoryCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public IStoryDTO Story
        {
            get => _story;

            set
            {
                _story = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            Preview.SetPictureUrl(Story.PreviewUrl, DefaultPlaylistPicture);
            Title.Text = Story.Title;
            if (Story.Objectionable){
                Title.TextColor = UIColor.LightGray;
                SelectionStyle = UITableViewCellSelectionStyle.None;
            }

            SenderPicture.SetPictureUrl(Story.SenderPictureUrl, DefaultUserPicture);
        }
    }
}