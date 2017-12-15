using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.iOS.Core;
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
        private IEventsBusinessLogic _businessLogic;
        private readonly List<EventDTO> _itemsList = new List<EventDTO>();
        private volatile bool _canLoadMore;
        private volatile bool _loadingMore;
        private UIImageView noItemsBackground;

        public EventsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            noItemsBackground = new UIImageView(UIImage.FromBundle("NoEvents"))
            {
                ContentMode = UIViewContentMode.Center
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = IoC.GetInstance<IEventsBusinessLogic>();
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
            SetTableBackground();
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

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._itemsList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (EventCell) tableView.DequeueReusableCell(EventCell.Key, indexPath);
            if (cell.Event == null)
            {
                cell.HostSelected = Cell_HostSelected;
                cell.AttendeeSelected = Cell_AttendeeSelected;
                cell.SendStoryButtonTouched = Cell_SendStory;
                cell.Touched = Cell_Touched;
                cell.UserInteractionEnabled = true;
            }

            cell.Event = this._itemsList[indexPath.Row];
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_itemsList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        public void DisplayItems(ICollection<EventDTO> items)
        {
            lock (((ICollection) _itemsList).SyncRoot)
            {
                var initialCount = _itemsList.Count;
                _itemsList.Clear();
                _itemsList.AddRange(items);

                this._canLoadMore = items.Count > initialCount;
            }

            InvokeOnMainThread(() =>
            {
                SetTableBackground();
                TableView.ReloadData();
            });
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public void ReloadItem(EventDTO dto)
        {
            var index = _itemsList.IndexOf(x => x.Id == dto.Id);
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

        private void Cell_SendStory(EventDTO eventDTO, EventCell cell)
        {
            _businessLogic.NavigateSendStory(eventDTO);
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

        private void SetTableBackground()
        {
            this.TableView.BackgroundView = this._itemsList.Count > 0 ? null : noItemsBackground;
        }
    }
}