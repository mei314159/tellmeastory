using System;

using Foundation;
using TellMe.Core.Contracts.DTO;
using TellMe.iOS.Core.UI;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class EventCell : UITableViewCell, IUICollectionViewDataSource, IUICollectionViewDelegate
    {
        public static readonly NSString Key = new NSString("EventCell");

        public static readonly UINib Nib;
        private EventDTO _event;
        private UIImage _defaultPicture;

        static EventCell()
        {
            Nib = UINib.FromName("EventCell", NSBundle.MainBundle);
        }

        protected EventCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        
        public Action<EventDTO, EventCell> HostSelected;

        public Action<EventDTO, EventCell> AcceptButtonTouched { get; set; }

        public Action<EventDTO, EventCell> SkipButtonTouched { get; set; }

        public Action<EventAttendeeDTO, EventCell> AttendeeSelected { get; set; }

        public EventDTO Event
        {
            get => _event;
            set
            {
                _event = value;
                this.Initialize();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


            this._defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
            AttendeesCollection.DelaysContentTouches = false;
            AttendeesCollection.RegisterNibForCell(EventAttendeesListCell.Nib, EventAttendeesListCell.Key);
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Event.Attendees?.Count ?? 0;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(EventAttendeesListCell.Key, indexPath) as EventAttendeesListCell;
            cell.Attendee = Event.Attendees[(int)indexPath.Item];
            return cell;
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath) as EventAttendeesListCell;
            if (cell != null)
            {
                this.AttendeeSelected?.Invoke(cell.Attendee, this);
            }
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Event.HostPictureUrl, _defaultPicture);
            this.Title.Text = Event.Title;
            this.Description.Text = Event.Description;
            this.DateDay.Text = Event.DateUtc.ToString("dd");
            this.DateMonth.Text = Event.DateUtc.ToString("MMM");
            AttendeesCollection.DataSource = this;
            AttendeesCollection.Delegate = this;
            AttendeesCollection.ReloadData();
        }
        
        private void ProfilePictureTouched()
        {
            this.HostSelected?.Invoke(Event, this);
        }

        partial void AcceptButton_TouchUpInside(Button sender)
        {
            this.AcceptButtonTouched?.Invoke(this.Event, this);
        }

        partial void SkipButton_TouchUpInside(Button sender)
        {
            this.SkipButtonTouched?.Invoke(this.Event, this);
        }
    }
}
