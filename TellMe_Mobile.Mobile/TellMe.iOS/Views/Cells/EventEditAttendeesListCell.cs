using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class EventEditAttendeesListCell : UITableViewCell
    {
        private UIImage _defaultPicture;
        public static readonly NSString Key = new NSString("EventEditAttendeesListCell");
        public static readonly UINib Nib;

        static EventEditAttendeesListCell()
        {
            Nib = UINib.FromName("EventEditAttendeesListCell", NSBundle.MainBundle);
        }

        protected EventEditAttendeesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this._defaultPicture = UIImage.FromBundle("UserPic");
        }

        private EventAttendeeDTO _attendee;

        public EventAttendeeDTO Attendee
        {
            get => _attendee;
            set
            {
                _attendee = value;
                this.Initialize();
            }
        }

        private void Initialize()
        {
            this.Username.Text = Attendee.AttendeeName;
            this.FullName.Text = Attendee.AttendeeFullName;
            this.ProfilePicture.SetPictureUrl(Attendee.AttendeePictureUrl, _defaultPicture);
            this.StatusLabel.Text = Attendee.Status.ToString();
        }
    }
}