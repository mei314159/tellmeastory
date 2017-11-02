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

        static StoriesListCell()
        {
            Nib = UINib.FromName("StoriesListCell", NSBundle.MainBundle);
        }

        protected StoriesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
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
