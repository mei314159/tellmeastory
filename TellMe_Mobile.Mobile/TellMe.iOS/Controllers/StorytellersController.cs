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
using System.Linq;
using TellMe.iOS.Views;

namespace TellMe.iOS
{
    public partial class StorytellersController : UIViewController, IStorytellersView, IUITableViewDataSource, IUITableViewDelegate
    {
        private StorytellersBusinessLogic _businessLogic;
        private List<ContactDTO> storytellersList = new List<ContactDTO>();
        private List<ContactDTO> tribesList = new List<ContactDTO>();
        private volatile bool loadingMore;
        private volatile bool canLoadMore;

        public ContactsMode Mode { get; set; }
        public event StorytellerSelectedEventHandler RecipientsSelected;
        public bool DismissOnFinish { get; set; }

        public string SearchText => SearchBar.Text;

        public HashSet<string> DisabledUserIds { get; set; }

        public StorytellersController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            this.SetMode(Mode);

            this._businessLogic = new StorytellersBusinessLogic(new RemoteStorytellersDataService(), new RemoteTribesDataService(), this, App.Instance.Router);
            this.TableView.RegisterNibForCellReuse(StorytellersListCell.Nib, StorytellersListCell.Key);
            this.TableView.RegisterNibForCellReuse(TribesListCell.Nib, TribesListCell.Key);
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
            UITapGestureRecognizer uITapGestureRecognizer = new UITapGestureRecognizer(HideSearchCancelButton);
            uITapGestureRecognizer.CancelsTouchesInView = false;
            this.View.AddGestureRecognizer(uITapGestureRecognizer);
            LoadAsync(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, true);
        }

        [Action("UnwindToStorytellers:")]
        public void UnwindToStorytellers(UIStoryboardSegue segue)
        {
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
            SearchBar.Text = null;
            SearchBar.EndEditing(true);
            LoadAsync(true);
        }

        private async void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            SearchBar.EndEditing(true);
            await LoadAsync(true);
        }

        public void DisplayContacts(ICollection<ContactDTO> contacts)
        {
            var initialCount = tribesList.Count + storytellersList.Count;
            lock (((ICollection)storytellersList).SyncRoot)
            {
                storytellersList.Clear();
                storytellersList.AddRange(contacts.Where(x => x.Type == ContactType.User).OrderBy(x => x.Name));
            }

            lock (((ICollection)tribesList).SyncRoot)
            {
                tribesList.Clear();
                tribesList.AddRange(contacts.Where(x => x.Type == ContactType.Tribe).OrderBy(x => x.Name));
            }

            this.canLoadMore = contacts.Count > initialCount;

            InvokeOnMainThread(() => TableView.ReloadData());
        }


        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return section == 0 ? this.storytellersList.Count : this.tribesList.Count;
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
            if (Mode == ContactsMode.FriendsAndTribes || Mode == ContactsMode.FriendsOnly)
            {
                NavItem.RightBarButtonItem.Enabled = tableView.IndexPathsForSelectedRows?.Length > 0;
                return;
            }

            tableView.DeselectRow(indexPath, false);
            if (indexPath.Section == 0)
            {
                var dto = this.storytellersList[indexPath.Row];
                string title = null;
                string confirmButtonTitle = null;
                switch (dto.User.FriendshipStatus)
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
            else if (indexPath.Section == 1)
            {
                var dto = this.tribesList[indexPath.Row];
                if (dto.Tribe.MembershipStatus == TribeMemberStatus.Joined || dto.Tribe.MembershipStatus == TribeMemberStatus.Creator)
                {
                    _businessLogic.ViewTribe(dto.Tribe);
                }
                else if (dto.Tribe.MembershipStatus == TribeMemberStatus.Invited)
                {
                    var alert = UIAlertController
                        .Create("Accept invitation to tribe?", string.Empty, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, (x) => RejectTribeInvitationTouched(dto)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, (x) => AcceptTribeInvitationTouched(dto)));
                    this.PresentViewController(alert, true, null);
                }
            }
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                var cell = tableView.DequeueReusableCell(StorytellersListCell.Key, indexPath) as StorytellersListCell;
                cell.Storyteller = this.storytellersList[indexPath.Row].User;
                cell.TintColor = UIColor.Blue;

                if (DisabledUserIds?.Contains(cell.Storyteller.Id) == true)
                {
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    cell.UserInteractionEnabled = false;
                }
                else
                {
                    cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
                    cell.UserInteractionEnabled = true;
                }

                return cell;
            }

            if (indexPath.Section == 1)
            {
                var cell = tableView.DequeueReusableCell(TribesListCell.Key, indexPath) as TribesListCell;
                cell.Tribe = this.tribesList[indexPath.Row].Tribe;
                cell.TintColor = UIColor.Blue;
                return cell;
            }

            return null;
        }

        [Export("tableView:editingStyleForRowAtIndexPath:")]
        public UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                var dto = storytellersList[indexPath.Row];
                if (DisabledUserIds?.Contains(dto.UserId) == true)
                    return UITableViewCellEditingStyle.None;
            }

            return (UITableViewCellEditingStyle)3;
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
            return Mode == ContactsMode.FriendsOnly ? 1 : 2;
        }

        partial void AddTribeButton_Activated(UIBarButtonItem sender)
        {
            _businessLogic.AddTribe();
        }

        private void SetMode(ContactsMode mode)
        {
            this.Mode = mode;
            switch (this.Mode)
            {
                case ContactsMode.Normal:
                    TableView.SetEditing(false, true);
                    NavItem.Title = "Storytellers";
                    TableViewTop.Constant = 44;
                    break;
                case ContactsMode.FriendsAndTribes:
                    NavItem.Title = "Choose recipient";
                    TableView.SetEditing(true, true);
                    SearchBar.Hidden = true;
                    TableViewTop.Constant = 0;
                    NavItem.SetRightBarButtonItem(new UIBarButtonItem("Continue", UIBarButtonItemStyle.Done, ContinueButtonTouched)
                    {
                        Enabled = false
                    }, false);
                    break;
                case ContactsMode.FriendsOnly:
                    NavItem.Title = "Choose Tribe Membes";
                    TableView.SetEditing(true, true);
                    SearchBar.Hidden = true;
                    TableViewTop.Constant = 0;
                    NavItem.SetRightBarButtonItem(new UIBarButtonItem("Continue", UIBarButtonItemStyle.Done, ContinueButtonTouched)
                    {
                        Enabled = false
                    }, false);
                    break;
            }
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

        private async void SendFriendshipRequest(ContactDTO contact)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.SendFriendshipRequestAsync(contact.User);
            TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(storytellersList.IndexOf(contact), 0) }, UITableViewRowAnimation.None);
            overlay.Close(true);
        }

        async void AcceptTribeInvitationTouched(ContactDTO contact)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.AcceptTribeInvitationAsync(contact.Tribe);
            TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(storytellersList.IndexOf(contact), 0) }, UITableViewRowAnimation.None);
            overlay.Close(true);
        }

        async void RejectTribeInvitationTouched(ContactDTO contact)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.RejectTribeInvitationAsync(contact.Tribe);
            TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(storytellersList.IndexOf(contact), 0) }, UITableViewRowAnimation.None);
            overlay.Close(true);
        }

        private async Task LoadAsync(bool forceRefresh)
        {
            var searchText = this.SearchBar.Text;
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh, searchText);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());

        }

        [Export("tableView:willDisplayCell:forRowAtIndexPath:")]
        public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (canLoadMore && (tribesList.Count + storytellersList.Count - indexPath.Row == 5))
            {
                LoadMoreAsync();
            }
        }

        private async Task LoadMoreAsync()
        {
            if (this.loadingMore)
                return;

            this.loadingMore = true;
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
            this.loadingMore = false;
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        private void ShowSendRequestToJoinPrompt()
        {
            var popup = InputPopupView.Create("Send request to join", "Please enter an email address to send the invitation", "Email address");
            popup.KeyboardType = UIKeyboardType.EmailAddress;
            popup.OnSubmit += async (email) => await _businessLogic.SendRequestToJoinPromptAsync(email);
            popup.PopUp(true);
        }

        void ContinueButtonTouched(object sender, EventArgs e)
        {
            var selectedContacts = TableView.IndexPathsForSelectedRows.Select(x => x.Section == 0 ? storytellersList[x.Row] : tribesList[x.Row]).ToList();
            RecipientsSelected?.Invoke(selectedContacts);
            if (DismissOnFinish)
            {
                if (NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewController(true, null);
            }
        }
    }
}