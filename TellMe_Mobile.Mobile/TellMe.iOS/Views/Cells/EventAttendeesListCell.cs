using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class EventAttendeesListCell : UICollectionViewCell
    {
        private EventAttendeeDTO attendee;
        private UIImage defaultPicture;

        public static readonly NSString Key = new NSString("EventAttendeesListCell");
        public static readonly UINib Nib;

        static EventAttendeesListCell()
        {
            Nib = UINib.FromName("EventAttendeesListCell", NSBundle.MainBundle);
        }

        protected EventAttendeesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        public EventAttendeeDTO Attendee
        {
            get { return attendee; }

            set
            {
                attendee = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            ProfilePicture.SetPictureUrl(attendee.AttendeePictureUrl, defaultPicture);
        }
    }
}