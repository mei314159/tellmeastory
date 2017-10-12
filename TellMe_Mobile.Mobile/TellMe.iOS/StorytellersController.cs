using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core;
using System.Collections;
using System.Threading.Tasks;
using TellMe.iOS.Views.Cells;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.iOS
{
    public partial class StorytellersController : UIViewController, IStorytellersView, IUITableViewDataSource
    {
        private StorytellersBusinessLogic _businessLogic;
        private List<StorytellerDTO> storytellersList = new List<StorytellerDTO>();

        public StorytellersController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            this._businessLogic = new StorytellersBusinessLogic(new RemoteStorytellersDataService(), this, App.Instance.Router);
            this.TableView.RegisterNibForCellReuse(StorytellersListCell.Nib, StorytellersListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.DataSource = this;
            this.SearchBar.OnEditingStarted += SearchBar_OnEditingStarted;
            this.SearchBar.OnEditingStopped += SearchBar_OnEditingStopped;
            this.SearchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            this.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;

            Task.Run(() => LoadStorytellers(false, true));
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

        private async void SearchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            SearchBar.EndEditing(true);
            Task.Run(() => LoadStorytellers(false));
        }

        private async void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            SearchBar.EndEditing(true);
            await SearchStorytellers(SearchBar.Text);
        }

        public void DisplayStorytellers(ICollection<StorytellerDTO> items)
        {
            lock (((ICollection)storytellersList).SyncRoot)
            {
                storytellersList.Clear();
                storytellersList.AddRange(items);
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

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return this.storytellersList.Count;
        }

        //public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        //{
        //    var cell = storytellersList[indexPath.Row];
        //    if (cell.Status == StoryStatus.Sent)
        //    {
        //        return tableView.Frame.Width + 64;
        //    }
        //    else
        //    {
        //        return 64;
        //    }
        //}

        //public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    var dto = this.storytellersList[indexPath.Row];
        //    if (dto.Status != StoryStatus.Requested || dto.ReceiverId == App.Instance.AuthInfo.UserId)
        //    {
        //        tableView.DeselectRow(indexPath, false);
        //    }
        //    else
        //    {
        //        businessLogic.SendStory(dto);
        //    }
        //}

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StorytellersListCell.Key, indexPath) as StorytellersListCell;
            cell.Storyteller = this.storytellersList[indexPath.Row];
            cell.OnSendFriendshiopRequestButtonTouched = SendFriendshipRequest;
            return cell;
        }

        private async void SendFriendshipRequest(StorytellerDTO storyteller)
        {
            await _businessLogic.SendFriendshipRequestAsync(storyteller);
            TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(storytellersList.IndexOf(storyteller), 0) }, UITableViewRowAnimation.None);
        }

        private async Task LoadStorytellers(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadStorytellersAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());

        }

        private async Task SearchStorytellers(string fragment)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.SearchStorytellersAsync(fragment);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }


        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStorytellers(true));
        }
    }
}