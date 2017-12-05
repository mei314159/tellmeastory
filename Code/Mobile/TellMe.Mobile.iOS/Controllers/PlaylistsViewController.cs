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
    public partial class PlaylistsViewController : UITableViewController, IPlaylistsView
    {
        private readonly IPlaylistsBusinessLogic _businessLogic;
        private readonly List<PlaylistDTO> _itemsList = new List<PlaylistDTO>();
        private volatile bool _canLoadMore;
        private volatile bool _loadingMore;

        public PlaylistsViewController(IPlaylistsBusinessLogic businessLogic) : base("PlaylistsViewController", null)
        {
            _businessLogic = businessLogic;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this._businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(PlaylistItemCell.Nib, PlaylistItemCell.Key);
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
            this.NavigationItem.Title = "Playlists";
            this.NavigationItem.RightBarButtonItem =
                new UIBarButtonItem(UIBarButtonSystemItem.Add, AddPlaylistButtonTouched);
            LoadItemsAsync(false, true);
        }

        private void AddPlaylistButtonTouched(object sender, EventArgs e)
        {
            this._businessLogic.CreatePlaylist();
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
            return this._itemsList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (PlaylistItemCell) tableView.DequeueReusableCell(PlaylistItemCell.Key, indexPath);
            cell.Playlist = this._itemsList[indexPath.Row];
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_itemsList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        public void DisplayItems(ICollection<PlaylistDTO> items)
        {
            lock (((ICollection) _itemsList).SyncRoot)
            {
                var initialCount = _itemsList.Count;
                _itemsList.Clear();
                _itemsList.AddRange(items);

                this._canLoadMore = items.Count > initialCount;
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public void ReloadItem(PlaylistDTO dto)
        {
            var index = _itemsList.IndexOf(x => x.Id == dto.Id);
            InvokeOnMainThread(() =>
                TableView.ReloadRows(new[] {NSIndexPath.FromRowSection(index, 0)}, UITableViewRowAnimation.None));
        }

        private async Task LoadItemsAsync(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadPlaylistsAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadItemsAsync(true));
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
            await _businessLogic.LoadPlaylistsAsync();
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this._loadingMore = false;
        }
    }
}