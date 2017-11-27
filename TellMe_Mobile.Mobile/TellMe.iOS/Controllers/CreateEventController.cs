using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MonoTouch.Dialog;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public class CreateEventController : DialogViewController, ICreateEventView
    {
        private readonly ICreateEventBusinessLogic _businessLogic;
        private CreateEventSource _dataSource;

        public CreateEventController(ICreateEventBusinessLogic businessLogic) : base(UITableViewStyle.Grouped, null,
            true)
        {
            _businessLogic = businessLogic;
            _businessLogic.View = this;
        }

        public event EventDeletedHandler EventDeleted;

        public EventDTO Event { get; set; }

        public override void ViewDidLoad()
        {
            var createEvent = Event == null;
            if (createEvent)
            {
                Event = new EventDTO
                {
                    Attendees = new List<EventAttendeeDTO>()
                };
            }

            ToggleRightButton(true);
            TableView.RefreshControl = new UIRefreshControl();
            TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.AllowsSelectionDuringEditing = true;
            TableView.SetEditing(true, true);
            this.Root = new RootElement("Edit Event")
            {
                new Section("Event Info")
                {
                    new EntryElement("Title", "Event Title", Event?.Title),
                    new EntryElement("Description", "Event Description", Event?.Description),
                    new DateElement("Date", createEvent ? DateTime.UtcNow : Event.DateUtc)
                },
                new Section("Members")
            };

            if (!createEvent)
                LoadAsync(false);
        }

        public override void Selected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Selected(indexPath);
        }

        public override void Deselected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Deselected(indexPath);
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            _dataSource = new CreateEventSource(this, Event);
            _dataSource.EditButtonTouched += DataSource_EditButtonTouched;
            _dataSource.OnDeleteRow += DataSource_OnDeleteRow;
            _dataSource.OnMemberSelected += DataSource_OnMemberSelected;
            return _dataSource;
        }

        private async Task LoadAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        private void ToggleRightButton(bool showButton)
        {
            InvokeOnMainThread(() =>
            {
                this.NavigationItem.RightBarButtonItem = showButton
                    ? new UIBarButtonItem("Continue", UIBarButtonItemStyle.Done, ContinueButtonTouched)
                    : null;
            });
        }

        private void DataSource_EditButtonTouched()
        {
            _businessLogic.ChooseMembers();
        }

        private void DataSource_OnDeleteRow(EventAttendeeDTO deletedItem, NSIndexPath indexPath)
        {
            Event.Attendees.Remove(deletedItem);
            TableView.DeleteRows(new[] {indexPath}, UITableViewRowAnimation.Automatic);
        }

        private void DataSource_OnMemberSelected(EventAttendeeDTO eventAttendee, NSIndexPath indexPath)
        {
            _businessLogic.NavigateAttendee(eventAttendee);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        public void Display(EventDTO eventDTO)
        {
            this.Event = eventDTO;
            InvokeOnMainThread(() =>
            {
                var root = this.Root[0];
                ((EntryElement) root[0]).Value = eventDTO.Title;
                ((EntryElement) root[1]).Value = eventDTO.Description;
                ((DateElement) root[2]).DateValue = eventDTO.CreateDateUtc;

                if (this.Root.Count == 2)
                {
                    var deleteEventButton = new UIButton(UIButtonType.System);
                    deleteEventButton.SetTitle("Delete Event", UIControlState.Normal);
                    deleteEventButton.TouchUpInside += DeleteEvent_TouchUpInside;
                    deleteEventButton.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44);
                    deleteEventButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
                    Root.Add(new Section(string.Empty)
                    {
                        new UIViewElement(string.Empty, deleteEventButton, false)
                    });
                }

                DisplayMembers();
            });
        }

        private void DeleteEvent_TouchUpInside(object sender, EventArgs e)
        {
            var alert = UIAlertController.Create(
                "Delete an event",
                "Do you really want to delete this event?",
                UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("Yes, I do", UIAlertActionStyle.Default,
                async obj => { await _businessLogic.DeleteEventAsync(); }));
            this.PresentViewController(alert, true, null);
        }

        public void DisplayMembers()
        {
            _dataSource.SetData(Event);
            TableView.ReloadData();
        }

        private void ContinueButtonTouched(object sender, EventArgs e)
        {
            var root = this.Root[0];
            TableView.RefreshControl.Enabled = true;
            Event.Title = ((EntryElement) root[0]).Value;
            Event.Description = ((EntryElement) root[1]).Value;
            Event.DateUtc = ((DateElement) root[2]).DateValue;
            _businessLogic.NavigateCreateRequest();
        }

        public void Close(EventDTO eventDTO)
        {
            EventDeleted?.Invoke(eventDTO);
            Dismiss();
        }

        public void Dismiss()
        {
            InvokeOnMainThread(() =>
            {
                if (NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewController(true, null);
            });
        }

        public IOverlay DisableInput()
        {
            ToggleRightButton(false);
            var overlay = new Overlay("Wait");
            overlay.PopUp();

            return overlay;
        }

        public void EnableInput(IOverlay overlay)
        {
            ToggleRightButton(true);
            overlay?.Close(false);
            overlay?.Dispose();
        }
    }

    public class CreateEventSource : DialogViewController.Source
    {
        private readonly List<EventAttendeeDTO> _membersList = new List<EventAttendeeDTO>();
        private UITableViewCell _addMemberCell;

        public event Action<EventAttendeeDTO, NSIndexPath> OnDeleteRow;
        public event Action<EventAttendeeDTO, NSIndexPath> OnMemberSelected;
        public event Action EditButtonTouched;

        public CreateEventSource(DialogViewController controller, EventDTO eventDTO) : base(controller)
        {
            controller.TableView.RegisterNibForCellReuse(EventEditAttendeesListCell.Nib,
                EventEditAttendeesListCell.Key);
            this.SetData(eventDTO);
        }

        public void SetData(EventDTO eventDTO)
        {
            lock (((ICollection) _membersList).SyncRoot)
            {
                _membersList.Clear();
                if (eventDTO.Attendees != null)
                    _membersList.AddRange(eventDTO.Attendees.OrderBy(x => x.AttendeeName));
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section != 1)
                return base.RowsInSection(tableview, section);
            return tableview.Editing ? _membersList.Count + 1 : _membersList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.GetCell(tableView, indexPath);

            if (tableView.Editing && indexPath.Row == 0)
            {
                if (_addMemberCell == null)
                {
                    _addMemberCell = new UITableViewCell();
                    _addMemberCell.TextLabel.Text = "Add Attendees";
                    _addMemberCell.TextLabel.TextAlignment = UITextAlignment.Center;
                    _addMemberCell.TextLabel.TextColor = _addMemberCell.DefaultTintColor();
                }

                return _addMemberCell;
            }

            var cell = (EventEditAttendeesListCell) tableView.DequeueReusableCell(EventEditAttendeesListCell.Key,
                indexPath);
            var index = tableView.Editing ? indexPath.Row - 1 : indexPath.Row;
            cell.Attendee = _membersList.ElementAt(index);
            return cell;
        }


        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
            {
                base.RowSelected(tableView, indexPath);
                return;
            }

            if (tableView.Editing && indexPath.Row == 0)
            {
                EditButtonTouched?.Invoke();
            }
            else
            {
                var cell = (EventEditAttendeesListCell) tableView.CellAt(indexPath);
                OnMemberSelected?.Invoke(cell.Attendee, indexPath);
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var result = indexPath.Section == 1 && indexPath.Row != 0;
            if (result)
            {
                var index = tableView.Editing ? indexPath.Row - 1 : indexPath.Row;
                var dto = _membersList[index];
                result = dto.Status != EventAttendeeStatus.Host;
            }

            return result;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.EditingStyleForRow(tableView, indexPath);

            return indexPath.Row == 0 ? UITableViewCellEditingStyle.None : UITableViewCellEditingStyle.Delete;
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete", DeleteRow);
            return new[] {deleteAction};
        }

        private void DeleteRow(UITableViewRowAction action, NSIndexPath indexPath)
        {
            var deletedItem = _membersList[(indexPath.Row - 1)];
            _membersList.Remove(deletedItem);
            OnDeleteRow?.Invoke(deletedItem, indexPath);
        }
    }
}