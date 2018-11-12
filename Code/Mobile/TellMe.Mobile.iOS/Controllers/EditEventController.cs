using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MonoTouch.Dialog;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public class EditEventController : DialogViewController, ICreateEventView
    {
        private readonly IEditEventBusinessLogic _businessLogic;
        private CreateEventSource _dataSource;
        private UIBarButtonItem _saveButton;
        private UIBarButtonItem _requestButton;

        public EditEventController(IEditEventBusinessLogic businessLogic) : base(UITableViewStyle.Grouped, null,
            true)
        {
            _businessLogic = businessLogic;
            _businessLogic.View = this;
        }

        public event ItemUpdateHandler<EventDTO> EventStateChanged;

        public EventDTO Event { get; set; }

        public bool CreateMode => Event == null || Event.Id == default(int);

        public override void ViewDidLoad()
        {
            if (CreateMode)
            {
                Event = new EventDTO
                {
                    Attendees = new List<EventAttendeeDTO>(),
                    DateUtc = DateTime.UtcNow
                };
            }

            _saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Done, SaveButtonTouched);
            _requestButton = new UIBarButtonItem("Request", UIBarButtonItemStyle.Done,
                (x, y) => _businessLogic.NavigateCreateRequest());

            ToggleRightButtons(true);
            TableView.RefreshControl = new UIRefreshControl();
            TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            DateElement dateElement = new DateElement("Date", Event.DateUtc.GetUtcDateTime());
            dateElement.DateSelected += DateElement_DateSelected;
            this.Root = new RootElement("Edit Event")
            {
                new Section("Event Info")
                {
                    new EntryElement("Title", "Event Title", Event?.Title),
                    new EntryElement("Description", "Event Description", Event?.Description),
                    dateElement,
                    new BooleanElement("ShareStories", Event?.ShareStories ?? false)
                }
            };
        }

        void DateElement_DateSelected(DateTimeElement dateTimeElement)
        {
            dateTimeElement.GetContainerTableView().ReloadRows(new[]{ NSIndexPath.FromRowSection(2, 0) }, UITableViewRowAnimation.Automatic);
        }


        public override void ViewWillAppear(bool animated)
        {
            this.Root.Caption = CreateMode ? "Create Event" : "Edit Event";

            if (!CreateMode)
            {
                this.Root.Add(new Section("Attendees"));
            }

            if (!CreateMode)
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
            _dataSource.OnMemberSelected += DataSource_OnMemberSelected;
            return _dataSource;
        }

        private async Task LoadAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        private void ToggleRightButtons(bool showButton)
        {
            InvokeOnMainThread(() =>
            {
                UIBarButtonItem[] buttons;
                if (showButton)
                    if (CreateMode)
                        buttons = new[]
                        {
                            this._saveButton
                        };
                    else
                        buttons = new[]
                        {
                            this._saveButton,
                            this._requestButton,
                        };

                else
                    buttons = new UIBarButtonItem[] { };

                this.NavigationItem.SetRightBarButtonItems(buttons, true);
            });
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
                ((EntryElement)root[0]).Value = eventDTO.Title;
                ((EntryElement)root[1]).Value = eventDTO.Description;
                ((DateElement)root[2]).DateValue = eventDTO.DateUtc.GetUtcDateTime();
                ((BooleanElement)root[3]).Value = eventDTO.ShareStories;
                if (!CreateMode && this.Root.Count == 2)
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

        private async void SaveButtonTouched(object sender, EventArgs e)
        {
            this.HideKeyboard();
            var root = this.Root[0];
            TableView.RefreshControl.Enabled = true;
            Event.Title = ((EntryElement)root[0]).Value;
            Event.Description = ((EntryElement)root[1]).Value;
            Event.DateUtc = ((DateElement)root[2]).DateValue;
            Event.ShareStories = ((BooleanElement)root[3]).Value;
            await _businessLogic.SaveAsync().ConfigureAwait(false);
        }

        public void Deleted(EventDTO eventDTO)
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

        public IOverlay DisableInput()
        {
            ToggleRightButtons(false);
            var overlay = new Overlay("Wait");
            overlay.PopUp();

            return overlay;
        }

        public void EnableInput(IOverlay overlay)
        {
            ToggleRightButtons(true);
            overlay?.Close(false);
            overlay?.Dispose();
        }

        public void PromptCreateRequest(EventDTO eventDTO)
        {
            InvokeOnMainThread(() =>
            {
                var alert = UIAlertController
                    .Create("Event Saved",
                        "Do you want to invite StoryTellers to your event?",
                        UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Not yet", UIAlertActionStyle.Cancel, x =>
                {
                    EventStateChanged?.Invoke(eventDTO, CreateMode ? ItemState.Created : ItemState.Updated);
                    ((IDismissable)this).Dismiss();
                }));
                alert.AddAction(UIAlertAction.Create("Yes, I do!", UIAlertActionStyle.Destructive,
                    x => _businessLogic.NavigateCreateRequest()));
                this.PresentViewController(alert, true, null);
            });
        }
    }

    public class CreateEventSource : DialogViewController.Source
    {
        private readonly List<EventAttendeeDTO> _membersList = new List<EventAttendeeDTO>();
        public event Action<EventAttendeeDTO, NSIndexPath> OnMemberSelected;

        public CreateEventSource(DialogViewController controller, EventDTO eventDTO) : base(controller)
        {
            controller.TableView.RegisterNibForCellReuse(EventEditAttendeesListCell.Nib,
                EventEditAttendeesListCell.Key);
            this.SetData(eventDTO);
        }

        public void SetData(EventDTO eventDTO)
        {
            lock (((ICollection)_membersList).SyncRoot)
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
            return _membersList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.GetCell(tableView, indexPath);

            var cell = (EventEditAttendeesListCell)tableView.DequeueReusableCell(EventEditAttendeesListCell.Key,
                indexPath);
            var index = indexPath.Row;
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

            var cell = (EventEditAttendeesListCell)tableView.CellAt(indexPath);
            OnMemberSelected?.Invoke(cell.Attendee, indexPath);
        }
    }
}