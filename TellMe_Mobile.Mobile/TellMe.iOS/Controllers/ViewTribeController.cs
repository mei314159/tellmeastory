using Foundation;
using UIKit;
using MonoTouch.Dialog;
using TellMe.iOS.Views.Cells;
using System;
using TellMe.Core.Contracts.DTO;
using System.Collections.Generic;
using System.Linq;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using System.Threading.Tasks;
using System.Collections;
using TellMe.Core.Contracts.UI;
using TellMe.Core;
using TellMe.iOS.Views;

namespace TellMe.iOS.Controllers
{
    public class ViewTribeController : DialogViewController, IViewTribeView
    {
        private readonly ViewTribeBusinessLogic _businessLogic;
        private ViewTribeSource DataSource;

        public event TribeCreatedHandler TribeCreated;

        public TribeDTO Tribe { get; set; }

        public ViewTribeController(TribeDTO tribe) : base(UITableViewStyle.Grouped, null, true)
        {
            this.Tribe = tribe;
            _businessLogic = new ViewTribeBusinessLogic(new RemoteTribesDataService(), App.Instance.Router, this);
        }

        public override void ViewDidLoad()
        {
            SetEditButton();
            TableView.RefreshControl = new UIRefreshControl();
            TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.Root = new RootElement(Tribe.Name) {
                new Section ("Tribe Info") {
                    new StringElement("Name", Tribe.Name),
                    new StringElement("Creator", Tribe.CreatorName),
                    new StringElement("Created", Tribe.CreateDateUtc.ToShortDateString()),
                    new StringElement("Your Status", Tribe.MembershipStatus.ToString())
                },
                new Section ("Members") {
                }
            };

            LoadAsync(false);
        }

        public override void Selected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Selected(indexPath);
        }

        public override void Deselected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Deselected(indexPath);
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            DataSource = new ViewTribeSource(this, Tribe);
            DataSource.EditButtonTouched += DataSource_EditButtonTouched;
            return DataSource;
        }

        public async Task LoadAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        void DataSource_EditButtonTouched()
        {
            _businessLogic.ChooseMembers();
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        public void Display(TribeDTO tribe)
        {
            this.Tribe = tribe;
            var root = this.Root[0];
            (root[0] as StringElement).Value = tribe.Name;
            (root[1] as StringElement).Value = tribe.CreatorName;
            (root[2] as StringElement).Value = tribe.CreateDateUtc.ToShortDateString();
            (root[3] as StringElement).Value = tribe.MembershipStatus.ToString();


            DataSource.SetData(tribe);
            InvokeOnMainThread(() =>
            {
                SetEditButton();
                TableView.ReloadData();
            });
        }

        private void SetEditButton()
        {
            if (Tribe.MembershipStatus == TribeMemberStatus.Creator)
            {
                this.NavigationItem.RightBarButtonItem = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Done, EditButtonTouched);
            }
            else
            {
                this.NavigationItem.RightBarButtonItem = null;
            }
        }

        private async void EditButtonTouched(object sender, EventArgs e)
        {
            var editing = !DataSource.Editing;
            this.NavigationItem.RightBarButtonItem.Title = editing ? "Done" : "Edit";
            var root = this.Root[0];
            if (editing)
            {
                this.NavigationItem.RightBarButtonItem.Title = "Done";
                TableView.RefreshControl.Enabled = false;
                root.Remove(0);
                root.Insert(0, new EntryElement("Name", "Enter Tribe Name", Tribe.Name));
            }
            else
            {
                this.NavigationItem.RightBarButtonItem.Title = "Edit";
                TableView.RefreshControl.Enabled = true;
                Tribe.Name = (root[0] as EntryElement).Value;
                root.Remove(0);
                root.Insert(0, new StringElement("Name", Tribe.Name));

                var overlay = new Overlay("Wait");
                overlay.PopUp(true);
                await _businessLogic.SaveAsync();
                overlay.Close(true);
            }

            InvokeOnMainThread(() =>
            {
                DataSource.Editing = editing;
                TableView.ReloadData();
            });
        }
    }

    public class ViewTribeSource : DialogViewController.Source
    {
        private readonly List<TribeMemberDTO> membersList = new List<TribeMemberDTO>();
        private UITableViewCell addMemberCell;
        public bool Editing { get; set; }

        public event Action EditButtonTouched;

        public ViewTribeSource(DialogViewController controller, TribeDTO tribe) : base(controller)
        {
            controller.TableView.RegisterNibForCellReuse(TribeMembersListCell.Nib, TribeMembersListCell.Key);
            this.SetData(tribe);
        }

        public void SetData(TribeDTO tribe)
        {
            var initialCount = membersList.Count;
            lock (((ICollection)membersList).SyncRoot)
            {
                membersList.Clear();
                if (tribe.Members != null)
                    membersList.AddRange(tribe.Members.OrderBy(x => x.UserName));
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
                return base.RowsInSection(tableview, section);
            return Editing ? membersList.Count + 1 : membersList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                return base.GetCell(tableView, indexPath);

            if (Editing && indexPath.Row == 0)
            {
                if (addMemberCell == null)
                {
                    addMemberCell = new UITableViewCell();
                    addMemberCell.TextLabel.Text = "Add Member";
                    addMemberCell.TextLabel.TextAlignment = UITextAlignment.Center;
                    addMemberCell.TextLabel.TextColor = UIColor.Blue;
                }

                return addMemberCell;
            }

            var cell = tableView.DequeueReusableCell(TribeMembersListCell.Key, indexPath) as TribeMembersListCell;
            var index = Editing ? indexPath.Row - 1 : indexPath.Row;
            cell.TribeMember = membersList.ElementAt(index);
            return cell;
        }


        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.RowSelected(tableView, indexPath);

            if (Editing && indexPath.Row == 0)
            {
                EditButtonTouched?.Invoke();
            }
            else
            {

            }
        }
    }
}