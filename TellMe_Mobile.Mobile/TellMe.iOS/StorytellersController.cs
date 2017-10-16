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
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class StorytellersController : UIViewController, IStorytellersView, IUITableViewDataSource, IUITableViewDelegate
    {
        private StorytellersBusinessLogic _businessLogic;
        private List<StorytellerDTO> storytellersList = new List<StorytellerDTO>();
        private List<StorytellerDTO> tribes = new List<StorytellerDTO>();

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
            this.TableView.Delegate = this;
            this.TableView.DataSource = this;
            this.SearchBar.OnEditingStarted += SearchBar_OnEditingStarted;
            this.SearchBar.OnEditingStopped += SearchBar_OnEditingStopped;
            this.SearchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
            this.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;
            UITapGestureRecognizer uITapGestureRecognizer = new UITapGestureRecognizer(HideSearchCancelButton);
            uITapGestureRecognizer.CancelsTouchesInView = false;
            this.View.AddGestureRecognizer(uITapGestureRecognizer);
            Task.Run(() => LoadStorytellers(false, true));
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, true);
        }

        void HideSearchCancelButton()
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

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
            if (indexPath.Section == 0)
            {
                var dto = this.storytellersList[indexPath.Row];
                string title = null;
                string confirmButtonTitle = null;
                switch (dto.FriendshipStatus)
                {
                    case FriendshipStatus.Accepted:
                    case FriendshipStatus.Rejected:
                    case FriendshipStatus.Requested:
                        return;
                    case FriendshipStatus.WaitingForResponse:
                        title = "Accept Follow Request?";
                        confirmButtonTitle = "Accept";
                        break;
                    case FriendshipStatus.None:
                        title = "Send a Follow Request?";
                        confirmButtonTitle = "Send";
                        break;

                }

                UIAlertController alert = UIAlertController
                    .Create(title, string.Empty, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Back", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create(confirmButtonTitle, UIAlertActionStyle.Default, (x) => SendFriendshipRequest(dto)));
                this.PresentViewController(alert, true, null);
            }
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StorytellersListCell.Key, indexPath) as StorytellersListCell;
            cell.Storyteller = this.storytellersList[indexPath.Row];
            return cell;
        }

        [Export("tableView:titleForHeaderInSection:")]
        public string TitleForHeader(UITableView tableView, nint section)
        {
            if (section == 0)
                return "Friends";
            if (section == 1)
                return "Tribes";

            return string.Empty;
        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return tribes.Count > 0 ? 2 : 1; //Friends Tribes
        }

        public void ShowSendRequestPrompt()
        {
            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController
                        .Create("Storytellers not found", "Send a request to join?", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Back", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Send", UIAlertActionStyle.Default, (x) => this.ShowSendRequestToJoinPrompt()));
                this.PresentViewController(alert, true, null);
            });
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

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
            {

                Task.Run(() => SearchStorytellers(SearchBar.Text));
            }
            else
            {
                Task.Run(() => LoadStorytellers(true));
            }

        }

        private void ShowSendRequestToJoinPrompt()
        {
            var popup = InputPopupView.Create("Send request to join", "Enter email address");
            popup.KeyboardType = UIKeyboardType.EmailAddress;
            popup.PopUp(true);
            popup.OnSubmit += async (email) => await _businessLogic.SendRequestToJoinPromptAsync(email);
        }
    }
}