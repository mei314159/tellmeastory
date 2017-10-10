using System;
using UIKit;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using Foundation;
using TellMe.iOS.Views.Cells;

namespace TellMe.iOS
{
    public partial class ContactDetailsViewController : UIViewController, IUITableViewDataSource, IContactDetailsView
    {
        private ContactDetailsBusinessLogic businessLogic;
        private List<StoryDTO> storiesList = new List<StoryDTO>();

        public ContactDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public ContactDTO ContactDTO { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.businessLogic = new ContactDetailsBusinessLogic(new RemoteStoriesDataService(), this);
            this.StoriesTableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.StoriesTableView.RowHeight = 64;
            this.StoriesTableView.DataSource = this;
            this.StoriesTableView.RefreshControl = new UIRefreshControl();
            this.StoriesTableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            Task.Run(() => LoadContactDetails(false));
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadContactDetails(true));
        }

        private async Task LoadContactDetails(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.StoriesTableView.RefreshControl.BeginRefreshing());
            await businessLogic.LoadContactDetails(forceRefresh);
            InvokeOnMainThread(() => this.StoriesTableView.RefreshControl.EndRefreshing());
        }

        public void DisplayContactDetails(ContactDTO dto)
        {
            InvokeOnMainThread(() =>
            {
                this.NavItem.Title = dto.Name;
            });
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection)storiesList).SyncRoot)
            {
                storiesList.Clear();
                storiesList.AddRange(stories);
            }

            InvokeOnMainThread(() => StoriesTableView.ReloadData());
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

        partial void RequestStoryButton_TouchUpInside(UIButton sender)
        {
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
            return this.storiesList.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this.storiesList[indexPath.Row];
            return cell;
        }
    }
}