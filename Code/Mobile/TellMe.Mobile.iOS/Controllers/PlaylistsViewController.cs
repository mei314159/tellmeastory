using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class PlaylistsViewController : UITableViewController, IPlaylistsView
    {
        private IPlaylistsBusinessLogic _businessLogic;
        private readonly List<PlaylistDTO> _itemsList = new List<PlaylistDTO>();
        private volatile bool _canLoadMore;
        private volatile bool _loadingMore;
        private UIBarButtonItem _doneButton;

        public PlaylistsViewController(IntPtr handle) : base(handle)
        {
        }

        public PlaylistViewMode Mode { get; set; }
        public Func<IDismissable, PlaylistDTO, Task> OnSelected;
        private UIImageView noItemsBackground;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            noItemsBackground = new UIImageView(UIImage.FromBundle("NoPlaylists"))
            {
                ContentMode = UIViewContentMode.Center
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _businessLogic = IoC.GetInstance<IPlaylistsBusinessLogic>();
            this._businessLogic.View = this;
            this._doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, DoneButtonTouched);
            this.TableView.RegisterNibForCellReuse(PlaylistItemCell.Nib, PlaylistItemCell.Key);
            this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.TableFooterView.Hidden = true;
            SetTableBackground();

            this.NavigationController.View.BackgroundColor = UIColor.White;
            this.NavigationItem.Title = "Playlists";
            this.NavigationItem.RightBarButtonItem =
                new UIBarButtonItem(UIBarButtonSystemItem.Add, AddPlaylistButtonTouched);

            if (Mode == PlaylistViewMode.SelectOne)
            {
                this.TableView.AllowsSelection = false;
                this.TableView.AllowsMultipleSelection = false;
                this.TableView.SetEditing(true, false);
                this.NavigationItem.SetRightBarButtonItems(new[]
                {
                    _doneButton,
                    this.NavigationItem.RightBarButtonItem
                }, false);
            }

            LoadItemsAsync(false, true);
        }

        private async void DoneButtonTouched(object sender, EventArgs e)
        {
            if (OnSelected != null)
            {
                var cell = (PlaylistItemCell)TableView.CellAt(TableView.IndexPathForSelectedRow);
                var overlay = new Overlay("Wait");
                overlay.PopUp();
                await OnSelected(this, cell.Playlist).ConfigureAwait(false);
                overlay.Close();
            }
        }

        private void AddPlaylistButtonTouched(object sender, EventArgs e)
        {
            this._businessLogic.CreatePlaylist();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._itemsList.Count;
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            if (Mode == PlaylistViewMode.SelectOne && tableView.IndexPathsForSelectedRows == null)
            {
                _doneButton.Enabled = false;
            }

            tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.None);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {

            if (Mode == PlaylistViewMode.Normal)
            {
                var cell = (PlaylistItemCell)tableView.CellAt(indexPath);
                _businessLogic.NavigateViewPlaylist(cell.Playlist);
                tableView.DeselectRow(indexPath, false);
            }
            else
            {
                _doneButton.Enabled = true;
                var deselect = tableView.IndexPathsForSelectedRows.Where(x => x != indexPath).ToList();
                foreach (var path in deselect)
                {
                    tableView.DeselectRow(path, false);
                }
            }
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return (UITableViewCellEditingStyle)3;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (PlaylistItemCell)tableView.DequeueReusableCell(PlaylistItemCell.Key, indexPath);
            cell.Playlist = this._itemsList[indexPath.Row];
            cell.TintColor = cell.DefaultTintColor();
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
            lock (((ICollection)_itemsList).SyncRoot)
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

        public void ReloadItem(PlaylistDTO dto)
        {
            var index = _itemsList.IndexOf(x => x.Id == dto.Id);
            InvokeOnMainThread(() =>
                TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.None));
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

        private void SetTableBackground()
        {
            this.TableView.BackgroundView = this._itemsList.Count > 0 ? null : noItemsBackground;
        }
    }
}