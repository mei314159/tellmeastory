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

namespace TellMe.iOS
{
    public partial class ContactsViewController : UITableViewController
    {
        private ContactsService contactsService;
        private List<ContactDTO> appUsers = new List<ContactDTO>();
        private List<ContactDTO> otherContacts = new List<ContactDTO>();

        public ContactsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.contactsService = new ContactsService(App.Instance.DataStorage);
            this.TableView.RegisterNibForCellReuse(ContactsListCell.Nib, ContactsListCell.Key);
            this.TableView.RowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            Task.Run(LoadContacts);
        }

        private async Task LoadContacts()
        {
			InvokeOnMainThread(() =>
			{
                this.TableView.RefreshControl.BeginRefreshing();
			});
            var contacts = await contactsService.GetContactsAsync();
            if (contacts.IsSuccess)
            {
                var groups = contacts.Data.GroupBy(x => x.IsAppUser)
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
                    this.TableView.RefreshControl.EndRefreshing();
                });
            }
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(LoadContacts);
        }

        partial void ImportButton_Activated(UIBarButtonItem sender)
        {
            this.View.Window.SwapController(UIStoryboard.FromName("Auth", null).InstantiateViewController("ImportContactsController"));
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
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
    }
}