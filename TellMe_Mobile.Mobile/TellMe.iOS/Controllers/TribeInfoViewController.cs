using Foundation;
using UIKit;
using MonoTouch.Dialog;
using TellMe.iOS.Views.Cells;
using System;
using TellMe.Core.Contracts.DTO;
using System.Collections.Generic;
using System.Linq;
using TellMe.iOS.Extensions;
using System.Threading.Tasks;
using System.Collections;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.Handlers;
using TellMe.iOS.Core;
using TellMe.iOS.Views;

namespace TellMe.iOS.Controllers
{
    public class TribeInfoViewController : DialogViewController, IViewTribeView
    {
        private readonly IViewTribeInfoBusinessLogic _businessLogic;
        private ViewTribeSource _dataSource;

        public event TribeLeftHandler TribeLeft;

        public TribeDTO Tribe { get; set; }

        public TribeInfoViewController(TribeDTO tribe) : base(UITableViewStyle.Grouped, null, true)
        {
            this.Tribe = tribe;
            this._businessLogic = IoC.GetInstance<IViewTribeInfoBusinessLogic>();
            _businessLogic.View = this;
        }

        public override void ViewDidLoad()
        {
            SetEditButton();
            TableView.RefreshControl = new UIRefreshControl();
            TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.AllowsSelectionDuringEditing = true;
            this.Root = new RootElement(Tribe.Name)
            {
                new Section("Tribe Info")
                {
                    new StringElement("Name", Tribe.Name),
                    new StringElement("Creator", Tribe.CreatorName),
                    new StringElement("Created", Tribe.CreateDateUtc.ToShortDateString()),
                    new StringElement("Your Status", Tribe.MembershipStatus.ToString())
                },
                new Section("Members")
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
            _dataSource = new ViewTribeSource(this, Tribe);
            _dataSource.EditButtonTouched += DataSource_EditButtonTouched;
            _dataSource.OnDeleteRow += DataSource_OnDeleteRow;
            _dataSource.OnMemberSelected += DataSource_OnMemberSelected;
            return _dataSource;
        }

        public async Task LoadAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        private void DataSource_EditButtonTouched()
        {
            _businessLogic.ChooseMembers();
        }

        private void DataSource_OnDeleteRow(TribeMemberDTO deletedItem, NSIndexPath indexPath)
        {
            Tribe.Members.Remove(deletedItem);
            TableView.DeleteRows(new[] {indexPath}, UITableViewRowAnimation.Automatic);
        }

        private void DataSource_OnMemberSelected(TribeMemberDTO tribeMember, NSIndexPath indexPath)
        {
            _businessLogic.NavigateTribeMember(tribeMember);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        public void Display(TribeDTO tribe)
        {
            this.Tribe = tribe;
            var root = this.Root[0];
            ((StringElement) root[0]).Value = tribe.Name;
            ((StringElement) root[1]).Value = tribe.CreatorName;
            ((StringElement) root[2]).Value = tribe.CreateDateUtc.ToShortDateString();
            ((StringElement) root[3]).Value = tribe.MembershipStatus.ToString();

            if (tribe.MembershipStatus == TribeMemberStatus.Joined)
            {
                InvokeOnMainThread(() =>
                {
                    if (this.Root.Count == 2)
                    {
                        UIButton leaveButton = new UIButton(UIButtonType.System);
                        leaveButton.SetTitle("Leave Tribe", UIControlState.Normal);
                        leaveButton.TouchUpInside += LeaveButton_TouchUpInside;
                        leaveButton.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44);
                        leaveButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
                        Root.Add(new Section(string.Empty)
                        {
                            new UIViewElement(string.Empty, leaveButton, false)
                        });
                    }
                });
            }

            InvokeOnMainThread(() =>
            {
                SetEditButton();
                DisplayMembers();
            });
        }

        private void LeaveButton_TouchUpInside(object sender, EventArgs e)
        {
            var alert = UIAlertController.Create(
                "Leave a Tribe",
                "Do you really want to leave this tribe?",
                UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("Yes, I do", UIAlertActionStyle.Default,
                async obj => { await _businessLogic.LeaveTribeAsync(); }));
            this.PresentViewController(alert, true, null);
        }

        public void DisplayMembers()
        {
            _dataSource.SetData(Tribe);
            TableView.ReloadData();
        }

        private void SetEditButton()
        {
            this.NavigationItem.RightBarButtonItem = Tribe.MembershipStatus == TribeMemberStatus.Creator
                ? new UIBarButtonItem("Edit", UIBarButtonItemStyle.Done, EditButtonTouched)
                : null;
        }

        private void EditButtonTouched(object sender, EventArgs e)
        {
            var editing = !TableView.Editing;
            this.NavigationItem.RightBarButtonItem.Title = editing ? "Done" : "Edit";
            var root = this.Root[0];
            if (editing)
            {
                this.NavigationItem.RightBarButtonItem.Title = "Done";
                TableView.RefreshControl.Enabled = false;
                root.Remove(0);
                root.Insert(0, new EntryElement("Name", "Enter Tribe Name", Tribe.Name));

                TableView.SetEditing(true, true);
            }
            else
            {
                this.NavigationItem.RightBarButtonItem.Title = "Edit";
                TableView.RefreshControl.Enabled = true;
                Tribe.Name = ((EntryElement) root[0]).Value;
                root.Remove(0);
                root.Insert(0, new StringElement("Name", Tribe.Name));
                TableView.SetEditing(false, false);
                var overlay = new Overlay("Wait");
                overlay.PopUp();
                InvokeInBackground(async () =>
                {
                    await _businessLogic.SaveAsync().ConfigureAwait(false);
                    overlay.Close();
                });
            }

            InvokeOnMainThread(() => { TableView.ReloadData(); });
        }

        public void Close(TribeDTO tribeLeft)
        {
            TribeLeft?.Invoke(tribeLeft);
            InvokeOnMainThread(() => { this.NavigationController.PopViewController(true); });
        }
    }

    public class ViewTribeSource : DialogViewController.Source
    {
        private readonly List<TribeMemberDTO> _membersList = new List<TribeMemberDTO>();
        private UITableViewCell _addMemberCell;

        public event Action<TribeMemberDTO, NSIndexPath> OnDeleteRow;

        public event Action<TribeMemberDTO, NSIndexPath> OnMemberSelected;

        public event Action EditButtonTouched;

        public ViewTribeSource(DialogViewController controller, TribeDTO tribe) : base(controller)
        {
            controller.TableView.RegisterNibForCellReuse(TribeMembersListCell.Nib, TribeMembersListCell.Key);
            this.SetData(tribe);
        }

        public void SetData(TribeDTO tribe)
        {
            lock (((ICollection) _membersList).SyncRoot)
            {
                _membersList.Clear();
                if (tribe.Members != null)
                    _membersList.AddRange(tribe.Members.OrderBy(x => x.UserName));
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section != 1)
                return base.RowsInSection(tableview, section);
            return tableview.Editing ? _membersList.Count + 1 : _membersList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.GetCell(tableView, indexPath);

            if (tableView.Editing && indexPath.Row == 0)
            {
                if (_addMemberCell == null)
                {
                    _addMemberCell = new UITableViewCell();
                    _addMemberCell.TextLabel.Text = "Add Member";
                    _addMemberCell.TextLabel.TextAlignment = UITextAlignment.Center;
                    _addMemberCell.TextLabel.TextColor = _addMemberCell.DefaultTintColor();
                }

                return _addMemberCell;
            }

            var cell = (TribeMembersListCell) tableView.DequeueReusableCell(TribeMembersListCell.Key, indexPath);
            var index = tableView.Editing ? indexPath.Row - 1 : indexPath.Row;
            cell.TribeMember = _membersList.ElementAt(index);
            return cell;
        }


        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
            {
                base.RowSelected(tableView, indexPath);
                return;
            }

            if (tableView.Editing && indexPath.Row == 0)
            {
                EditButtonTouched?.Invoke();
            }
            else
            {
                var cell = (TribeMembersListCell) tableView.CellAt(indexPath);
                OnMemberSelected?.Invoke(cell.TribeMember, indexPath);
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var result = indexPath.Section == 1 && indexPath.Row != 0;
            if (result)
            {
                var index = tableView.Editing ? indexPath.Row - 1 : indexPath.Row;
                var dto = _membersList[index];
                result = dto.Status != TribeMemberStatus.Creator;
            }

            return result;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.EditingStyleForRow(tableView, indexPath);

            return indexPath.Row == 0 ? UITableViewCellEditingStyle.None : UITableViewCellEditingStyle.Delete;
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete", DeleteRow);
            return new[] {deleteAction};
        }

        private void DeleteRow(UITableViewRowAction action, NSIndexPath indexPath)
        {
            var deletedItem = _membersList[(indexPath.Row - 1)];
            _membersList.Remove(deletedItem);
            OnDeleteRow?.Invoke(deletedItem, indexPath);
        }
    }
}