using System;
using Foundation;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Shared.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class SlimStoryCell : UITableViewCell
    {
        private StoryListDTO _story;

        public static readonly NSString Key = new NSString("SlimStoryCell");
        public static readonly UINib Nib;
        private static readonly UIImage DefaultPicture;

        static SlimStoryCell()
        {
            Nib = UINib.FromName("SlimStoryCell", NSBundle.MainBundle);
            DefaultPicture = UIImage.FromBundle("UserPic");
        }

        protected SlimStoryCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public StoryListDTO Story
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
            Preview.SetPictureUrl(Story.PreviewUrl, DefaultPicture);
            Title.Text = Story.Title;
            SenderPicture.SetPictureUrl(Story.SenderPictureUrl, DefaultPicture);
        }
    }
}