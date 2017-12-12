using System;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class EventViewController : StoriesTableViewController, IEventView
    {
        private EventCell _eventCell;

        private new IEventViewBusinessLogic BusinessLogic
        {
            get => (IEventViewBusinessLogic) base.BusinessLogic;
            set => base.BusinessLogic = value;
        }

        public EventViewController(IntPtr handle) : base(handle)
        {
        }

        public event ItemUpdateHandler<EventDTO> EventStateChanged;
        
        public EventDTO Event { get; set; }
        public int EventId { get; set; }

        public override int StoryItemIndexOffset => _eventCell == null ? 0 : 1;

        public override void ViewDidLoad()
        {
            if (this.BusinessLogic == null)
                this.BusinessLogic = IoC.GetInstance<IEventViewBusinessLogic>();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, false);
        }

        public void DisplayEvent(EventDTO eventDTO, bool canEdit)
        {
            InvokeOnMainThread(() =>
            {

                this.NavigationItem.RightBarButtonItem = canEdit
                    ? new UIBarButtonItem(UIBarButtonSystemItem.Edit, (s, e) => this.BusinessLogic.EditEvent())
                    : null;
                if (_eventCell == null)
                {
                    _eventCell = EventCell.Create();
                    _eventCell.HostSelected = Cell_HostSelected;
                    _eventCell.AttendeeSelected += Cell_AttendeeSelected;
                    _eventCell.UserInteractionEnabled = true;
                }

                _eventCell.Event = eventDTO;
            });
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_eventCell != null && indexPath.Row == 0)
            {
                return _eventCell;
            }

            return base.GetCell(tableView, indexPath);
        }

        void IEventView.EventDeleted(EventDTO eventDTO)
        {
            EventStateChanged?.Invoke(eventDTO, ItemState.Deleted);
            ((IDismissable)this).Dismiss();
        }
        
        void IDismissable.Dismiss()
        {
            InvokeOnMainThread(() =>
            {
                if (NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewController(true, null);
            });
        }

        private void Cell_HostSelected(EventDTO eventDTO, EventCell cell)
        {
            BusinessLogic.NavigateStoryteller(eventDTO.HostId);
        }

        private void Cell_AttendeeSelected(EventAttendeeDTO eventAttendee, EventCell cell)
        {
            if (eventAttendee.TribeId == null)
            {
                BusinessLogic.NavigateStoryteller(eventAttendee.UserId);
            }
            else
            {
                BusinessLogic.NavigateTribe(eventAttendee.TribeId.Value, cell.RemoveTribe);
            }
        }
    }
}