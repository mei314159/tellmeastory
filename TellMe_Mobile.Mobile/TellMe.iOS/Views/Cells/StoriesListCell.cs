using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;
namespace TellMe.iOS.Views.Cells
{
    public partial class StoriesListCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("StoriesListCell");
        public static readonly UINib Nib;

        public Action<StoryDTO> ProfilePictureTouched { get; set; }
        public Action<StoryDTO> PreviewTouched { get; set; }

        static StoriesListCell()
        {
            Nib = UINib.FromName("StoriesListCell", NSBundle.MainBundle);
        }

        protected StoriesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.StoryView.OnProfilePictureTouched += StoryView_OnProfilePictureTouched;
            this.StoryView.OnPreviewTouched += StoryView_OnPreviewTouched;
        }

        void StoryView_OnProfilePictureTouched(StoryDTO story)
        {
            ProfilePictureTouched?.Invoke(story);
        }

        void StoryView_OnPreviewTouched(StoryDTO story)
        {
            PreviewTouched?.Invoke(story);
        }

        public StoryDTO Story
        {
            get => StoryView.Story;
            set => StoryView.Story = value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StoryView.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
