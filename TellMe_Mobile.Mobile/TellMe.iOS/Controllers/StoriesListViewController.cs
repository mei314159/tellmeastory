using Foundation;
using System;
using UIKit;
using TellMe.Core.Contracts.UI.Views;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using System.Collections;
using TellMe.Core.Types.BusinessLogic;
using System.Threading.Tasks;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Views.Cells;
using TellMe.Core;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class StoriesListViewController : UITableViewController, IStoriesListView //, IUITableViewDataSourcePrefetching
    {
        private StoriesBusinessLogic _businessLogic;
        private List<StoryDTO> storiesList = new List<StoryDTO>();
        private volatile bool loadingMore;
        private volatile bool canLoadMore;


        public StoriesListViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = new StoriesBusinessLogic(new RemoteStoriesDataService(), this, App.Instance.Router);
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.TableView.AllowsSelection = false;
            this.NavigationController.View.BackgroundColor = UIColor.White;

            Task.Run(() => LoadStoriesAsync(false, true));

            ((AppDelegate)UIApplication.SharedApplication.Delegate).CheckPushNotificationsPermissions();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.SetToolbarItems(new[]{
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem("Request a Story", UIBarButtonItemStyle.Plain, RequestStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace),
                new UIBarButtonItem("Send a Story", UIBarButtonItemStyle.Plain, SendStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            }, true);
            this.NavigationController.SetToolbarHidden(false, true);
        }

        [Action("UnwindToStoriesViewController:")]
        public void UnwindToStoriesViewController(UIStoryboardSegue segue)
        {
            Task.Run(() => LoadStoriesAsync(true, false));
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection)storiesList).SyncRoot)
            {
                var initialCount = storiesList.Count;
                storiesList.Clear();
                storiesList.AddRange(stories);

                this.canLoadMore = stories.Count > initialCount;
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);
        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.storiesList.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (storiesList.Count == 0 || indexPath.Row >= storiesList.Count)
                return 64;

            return tableView.Frame.Width + 64;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this.storiesList[indexPath.Row];
            cell.ProfilePictureTouched = Cell_OnProfilePictureTouched;
            cell.PreviewTouched = Cell_OnPreviewTouched;
            cell.UserInteractionEnabled = true;
            return cell;
        }

        void Cell_OnPreviewTouched(StoryDTO story)
        {
            _businessLogic.ViewStory(story);
        }

        void Cell_OnProfilePictureTouched(StoryDTO story)
        {
            _businessLogic.NavigateStoryteller(story);
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (storiesList.Count - indexPath.Row == 5 && canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        private async Task LoadMoreAsync()
        {
            if (this.loadingMore)
                return;

            this.loadingMore = true;
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StartAnimating();
                this.TableView.TableFooterView.Hidden = false;
            });
            await _businessLogic.LoadStoriesAsync(false, false);
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this.loadingMore = false;
        }

        private async Task LoadStoriesAsync(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadStoriesAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStoriesAsync(true));
        }

        void SendStoryButtonTouched(object sender, EventArgs e)
        {
            _businessLogic.SendStory();
        }

        void RequestStoryButtonTouched(object sender, EventArgs e)
        {
            _businessLogic.RequestStory();
        }

        partial void AccountSettingsButton_Activated(UIBarButtonItem sender)
        {
            _businessLogic.AccountSettings();
        }

        partial void Notifications_Activated(UIBarButtonItem sender)
        {
            _businessLogic.NotificationsCenter();
        }

        partial void Storytellers_Activated(UIBarButtonItem sender)
        {
            _businessLogic.ShowStorytellers();
        }
    }
}