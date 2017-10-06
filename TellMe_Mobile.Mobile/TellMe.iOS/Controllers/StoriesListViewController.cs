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

            Task.Run(() => LoadStories(false, true));

            ((AppDelegate)UIApplication.SharedApplication.Delegate).CheckPushNotificationsPermissions();
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

        public void ShowErrorMessage(string title, string message = null)
        {
            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController
                    .Create(title,
                            message ?? string.Empty,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                this.PresentViewController(alert, true, null);
            });
        }

        public void ShowSuccessMessage(string message)
        {
            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController
                    .Create("Success",
                            message ?? string.Empty,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                this.PresentViewController(alert, true, null);
            });
        }

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
            var dto = this.storiesList[indexPath.Row];
            if (dto.Status != StoryStatus.Requested || dto.ReceiverId == App.Instance.AuthInfo.UserId)
            {
                tableView.DeselectRow(indexPath, false);
            }
            else
            {
                businessLogic.SendStory(dto);
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this.storiesList[indexPath.Row];
            return cell;
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

        partial void SendStoryButtonTouched(UIBarButtonItem sender)
        {
            businessLogic.SendStory();
        }

        partial void RequestStoryButtonTouched(UIBarButtonItem sender)
        {
            businessLogic.RequestStory();
        }
    }
}