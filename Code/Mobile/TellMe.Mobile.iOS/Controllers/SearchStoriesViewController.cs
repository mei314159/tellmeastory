using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.iOS.Views.Cells;
using TellMe.iOS.Extensions;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using TellMe.iOS.Core;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.iOS
{
    public partial class SearchStoriesViewController : UIViewController, ISearchStoriesView, IUITableViewDataSource,
        IUITableViewDelegate
    {
        private readonly List<StoryListDTO> _storiesList = new List<StoryListDTO>();
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;
        private ISearchStoriesBusinessLogic _businessLogic;

        public SearchStoriesViewController(IntPtr handle) : base(handle)
        {
        }

        public bool DismissOnFinish { get; set; }

        public HashSet<int> DisabledStoryIds { get; set; }

        public event ItemsSelectedHandler<StoryListDTO> ItemsSelected;

        public override void ViewDidLoad()
        {
            NavItem.Title = "Choose stories";
            TableView.SetEditing(true, true);
            NavItem.SetRightBarButtonItem(
                        new UIBarButtonItem("Continue", UIBarButtonItemStyle.Done, ContinueButtonTouched)
                        {
                            Enabled = false
                        }, false);

            this._businessLogic = IoC.GetInstance<ISearchStoriesBusinessLogic>();
            _businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(SlimStoryCell.Nib, SlimStoryCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.Delegate = this;
            this.TableView.DataSource = this;
            this.SearchBar.OnEditingStarted += SearchBar_OnEditingStarted;
            this.SearchBar.OnEditingStopped += SearchBar_OnEditingStopped;
            this.SearchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            this.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;
            this.TableView.TableFooterView.Hidden = true;
            UITapGestureRecognizer uITapGestureRecognizer = new UITapGestureRecognizer(HideSearchCancelButton)
            {
                CancelsTouchesInView = false
            };
            uITapGestureRecognizer.CancelsTouchesInView = false;
            this.View.AddGestureRecognizer(uITapGestureRecognizer);
            LoadAsync(false);
        }

        public void DisplayStories(ICollection<StoryListDTO> items)
        {
            var initialCount = _storiesList.Count;
            lock (((ICollection)_storiesList).SyncRoot)
            {
                _storiesList.Clear();
                _storiesList.AddRange(items.OrderByDescending(x => x.CreateDateUtc));
            }

            this._canLoadMore = items.Count > initialCount;

            InvokeOnMainThread(() => TableView.ReloadData());
        }


        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return this._storiesList.Count;
        }

        [Export("tableView:didDeselectRowAtIndexPath:")]
        public void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView.IndexPathsForSelectedRows == null)
            {
                NavItem.RightBarButtonItem.Enabled = false;
            }

            var cell = tableView.CellAt(indexPath);
            tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.None);
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            NavItem.RightBarButtonItem.Enabled = tableView.IndexPathsForSelectedRows?.Length > 0;
        }

        [Export("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var dto = _storiesList[indexPath.Row];
            return DisabledStoryIds?.Contains(dto.Id) != true;

        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SlimStoryCell) tableView.DequeueReusableCell(SlimStoryCell.Key, indexPath);
            cell.Story = this._storiesList[indexPath.Row];
            cell.TintColor = UIColor.Blue;

            return cell;
        }

        [Export("tableView:editingStyleForRowAtIndexPath:")]
        public UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var dto = _storiesList[indexPath.Row];
            if (DisabledStoryIds?.Contains(dto.Id) == true)
                return UITableViewCellEditingStyle.None;

            return (UITableViewCellEditingStyle)3;
        }

        [Export("tableView:willDisplayCell:forRowAtIndexPath:")]
        public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_canLoadMore && (_storiesList.Count - indexPath.Row == 5))
            {
                LoadMoreAsync();
            }
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

        private async Task LoadMoreAsync()
        {
            if (this._loadingMore)
                return;

            this._loadingMore = true;
            var searchText = this.SearchBar.Text;
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StartAnimating();
                this.TableView.TableFooterView.Hidden = false;
            });
            await _businessLogic.LoadAsync(false, searchText);
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });
            this._loadingMore = false;
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        private void HideSearchCancelButton()
        {
            SearchBar.EndEditing(true);
        }

        private void SearchBar_OnEditingStarted(object sender, EventArgs e)
        {
            SearchBar.SetShowsCancelButton(true, true);
        }

        private void SearchBar_OnEditingStopped(object sender, EventArgs e)
        {
            SearchBar.SetShowsCancelButton(false, true);
            SearchBar.ResignFirstResponder();
        }

        private void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            SearchBar.Text = null;
            SearchBar.EndEditing(true);
            LoadAsync(true);
        }

        private async void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            SearchBar.EndEditing(true);
            await LoadAsync(true);
        }

        private void ContinueButtonTouched(object sender, EventArgs e)
        {
            var selectedStories = TableView.IndexPathsForSelectedRows.Select(x => _storiesList[x.Row]).ToList();
            ItemsSelected?.Invoke(this, selectedStories);
            if (DismissOnFinish)
            {
                Dismiss();
            }
        }

        private async Task LoadAsync(bool forceRefresh)
        {
            var searchText = this.SearchBar.Text;
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh, searchText);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }
    }
}