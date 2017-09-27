using Foundation;
using System;
using UIKit;
using TellMe.iOS.Extensions;
using TellMe.Core;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using TellMe.iOS.Views.Cells;
using System.Linq;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.iOS
{
    public partial class ContactsViewController : UITableViewController, IContactsView
    {
        private ContactsBusinessLogic businessLogic;
        private List<ContactDTO> appUsers = new List<ContactDTO>();
        private List<ContactDTO> otherContacts = new List<ContactDTO>();

        public ContactsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.businessLogic = new ContactsBusinessLogic(new RemoteContactsDataService(), this);
            this.TableView.RegisterNibForCellReuse(ContactsListCell.Nib, ContactsListCell.Key);
            this.TableView.RowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;

            Task.Run(() => LoadContacts(false));
        }

        private async Task LoadContacts(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await businessLogic.LoadContactsAsync(forceRefresh);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadContacts(true));
        }

        partial void ImportButton_Activated(UIBarButtonItem sender)
        {
            this.View.Window.SwapController(UIStoryboard.FromName("Auth", null).InstantiateViewController("ImportContactsController"));
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var dto = (indexPath.Section == 0 ? appUsers : otherContacts)[indexPath.Row];


        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return section == 0 ? "App users" : "Other contacts";
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return section == 0 ? appUsers.Count : otherContacts.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(ContactsListCell.Key, indexPath) as ContactsListCell;
            cell.Contact = (indexPath.Section == 0 ? appUsers : otherContacts)[indexPath.Row];
            return cell;
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

        public void DisplayContacts(ICollection<ContactDTO> contacts)
        {
            var groups = contacts.GroupBy(x => x.IsAppUser)
                                 .ToDictionary(x => x.Key, x => x.ToList());
            lock (((ICollection)appUsers).SyncRoot)
            {
                appUsers.Clear();
                if (groups.ContainsKey(true))
                    appUsers.AddRange(groups[true]);
            }

            lock (((ICollection)otherContacts).SyncRoot)
            {
                otherContacts.Clear();
                if (groups.ContainsKey(false))
                    otherContacts.AddRange(groups[false]);
            }

            InvokeOnMainThread(() =>
            {
                TableView.ReloadData();
            });
        }
    }
}