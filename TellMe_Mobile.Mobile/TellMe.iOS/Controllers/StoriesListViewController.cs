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
        private StoriesBusinessLogic businessLogic;
        private List<StoryDTO> storiesList = new List<StoryDTO>();

        public StoriesListViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.businessLogic = new StoriesBusinessLogic(new RemoteStoriesDataService(), this, App.Instance.Router);
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.NavigationController.View.BackgroundColor = UIColor.White;

            this.SetToolbarItems(new[]{
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem("Request a Story", UIBarButtonItemStyle.Plain, RequestStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace),
                new UIBarButtonItem("Send a Story", UIBarButtonItemStyle.Plain, SendStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            }, true);
            Task.Run(() => LoadStories(false, true));

            ((AppDelegate)UIApplication.SharedApplication.Delegate).CheckPushNotificationsPermissions();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(false, true);
        }

        [Action("UnwindToStoriesViewController:")]
        public void UnwindToStoriesViewController(UIStoryboardSegue segue)
        {
            Task.Run(() => LoadStories(true, false));
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection)storiesList).SyncRoot)
            {
                storiesList.Clear();
                storiesList.AddRange(stories);
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
            var cell = storiesList[indexPath.Row];
            if (cell.Status == StoryStatus.Sent)
            {
                return tableView.Frame.Width + 64;
            }
            else
            {
                return 64;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //var dto = this.storiesList[indexPath.Row];
            //if (dto.Status != StoryStatus.Requested || dto.ReceiverId == App.Instance.AuthInfo.UserId)
            //{
            //    tableView.DeselectRow(indexPath, false);
            //}
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this.storiesList[indexPath.Row];
            return cell;
        }

        public override void CellDisplayingEnded(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            (cell as StoriesListCell)?.EndDisplaying();
        }

        private async Task LoadStories(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await businessLogic.LoadStoriesAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());

        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStories(true));
        }

        void SendStoryButtonTouched(object sender, EventArgs e)
        {
            businessLogic.SendStory();
        }

        void RequestStoryButtonTouched(object sender, EventArgs e)
        {
            businessLogic.RequestStory();
        }

        partial void AccountSettingsButton_Activated(UIBarButtonItem sender)
        {
            businessLogic.AccountSettings();
        }

        partial void Notifications_Activated(UIBarButtonItem sender)
        {
            businessLogic.NotificationsCenter();
        }

        partial void Storytellers_Activated(UIBarButtonItem sender)
        {
            businessLogic.ShowStorytellers();
        }
    }
}