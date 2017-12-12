using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class EventsViewController : UITableViewController, IEventsView
    {
        private readonly IEventsBusinessLogic _businessLogic;
        private readonly List<EventDTO> _eventsList = new List<EventDTO>();
        private volatile bool _canLoadMore;
        private volatile bool _loadingMore;

        public EventsViewController(IEventsBusinessLogic businessLogic) : base("EventsViewController", null)
        {
            _businessLogic = businessLogic;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this._businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(EventCell.Nib, EventCell.Key);
            this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.TableView.AllowsSelection = false;
            this.NavigationController.View.BackgroundColor = UIColor.White;
            this.NavigationItem.Title = "Events";
            this.NavigationItem.RightBarButtonItem =
                new UIBarButtonItem(UIBarButtonSystemItem.Add, AddEventButtonTouched);
            LoadEventsAsync(false, true);
        }

        private void AddEventButtonTouched(object sender, EventArgs e)
        {
            this._businessLogic.CreateEvent();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, true);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._eventsList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (EventCell) tableView.DequeueReusableCell(EventCell.Key, indexPath);
            if (cell.Event == null)
            {
                cell.HostSelected = Cell_HostSelected;
                cell.AttendeeSelected = Cell_AttendeeSelected;
                cell.Touched = Cell_Touched;
                cell.UserInteractionEnabled = true;
            }

            cell.Event = this._eventsList[indexPath.Row];
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_eventsList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        public void DisplayItems(ICollection<EventDTO> items)
        {
            lock (((ICollection) _eventsList).SyncRoot)
            {
                var initialCount = _eventsList.Count;
                _eventsList.Clear();
                _eventsList.AddRange(items);

                this._canLoadMore = items.Count > initialCount;
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public void ReloadItem(EventDTO dto)
        {
            var index = _eventsList.IndexOf(x => x.Id == dto.Id);
            InvokeOnMainThread(() =>
                TableView.ReloadRows(new[] {NSIndexPath.FromRowSection(index, 0)}, UITableViewRowAnimation.None));
        }

        private async Task LoadEventsAsync(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadEventsAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadEventsAsync(true));
        }

        private async Task LoadMoreAsync()
        {
            if (this._loadingMore)
                return;

            this._loadingMore = true;
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StartAnimating();
                this.TableView.TableFooterView.Hidden = false;
            });
            await _businessLogic.LoadEventsAsync();
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this._loadingMore = false;
        }

        private void Cell_Touched(EventDTO eventDTO, EventCell cell)
        {
            _businessLogic.NavigateViewEvent(eventDTO);
        }

        private void Cell_HostSelected(EventDTO eventDTO, EventCell cell)
        {
            _businessLogic.NavigateStoryteller(eventDTO.HostId);
        }

        private void Cell_AttendeeSelected(EventAttendeeDTO eventAttendee, EventCell cell)
        {
            if (eventAttendee.TribeId == null)
            {
                _businessLogic.NavigateStoryteller(eventAttendee.UserId);
            }
            else
            {
                _businessLogic.NavigateTribe(eventAttendee.TribeId.Value, cell.RemoveTribe);
            }
        }
    }
}