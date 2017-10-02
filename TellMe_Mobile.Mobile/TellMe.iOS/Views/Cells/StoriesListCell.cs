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
        StoryDTO story;

        public StoryDTO Story
        {
            get
            {
                return story;
            }

            set
            {
                story = value;
                this.Initialize();
            }
        }

        private void Initialize()
		{
            this.Title.Text = Story.Title;
            this.Description.Text = Story.Description;
            this.Date.Text = Story.RequestDateUtc.ToShortDateString();
		}
    }
}
