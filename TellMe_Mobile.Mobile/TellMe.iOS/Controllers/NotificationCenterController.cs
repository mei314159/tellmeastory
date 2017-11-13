using Foundation;
using System;
using UIKit;
using TellMe.Core.Types.DataServices.Remote;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.iOS.Views.Cells;
using TellMe.Core.DTO;
using TellMe.Core.Types.BusinessLogic;
using System.Threading.Tasks;
using System.Collections;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using TellMe.Core;
using TellMe.iOS.Core;

namespace TellMe.iOS
{
    public partial class NotificationCenterController : UIViewController, IUITableViewDelegate, IUITableViewDataSource, INotificationsCenterView
    {
        private readonly List<NotificationDTO> notificationsList = new List<NotificationDTO>();

        private NotificationCenterBusinessLogic businessLogic;

        public NotificationCenterController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            businessLogic = new NotificationCenterBusinessLogic(
                new NotificationHandler(App.Instance.Router, this),
                new RemoteNotificationsDataService(),
                this);
            this.TableView.TableFooterView = new UIView();
            this.TableView.DataSource = this;
            this.TableView.Delegate = this;
            this.TableView.RegisterNibForCellReuse(NotificationCenterCell.Nib, NotificationCenterCell.Key);
            this.TableView.RegisterNibForCellReuse(NotificationCenterStoryCell.Nib, NotificationCenterStoryCell.Key);
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += (s, e) => LoadNotificationsAsync(true);

            Task.Run(() => LoadNotificationsAsync(false));
        }

        private async Task LoadNotificationsAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => TableView.RefreshControl.BeginRefreshing());
            await businessLogic.LoadNotificationsAsync(forceRefresh).ConfigureAwait(false);
            InvokeOnMainThread(() => TableView.RefreshControl.EndRefreshing());
        }

        public void DisplayNotifications(IReadOnlyCollection<NotificationDTO> notifications)
        {
            lock (((ICollection)notificationsList).SyncRoot)
            {
                notificationsList.Clear();
                notificationsList.AddRange(notifications);
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return notificationsList.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var dto = notificationsList[indexPath.Row];
            INotificationCenterCell cell;
            if (dto.Type == NotificationTypeEnum.Story)
            {
                cell = (NotificationCenterStoryCell)tableView.DequeueReusableCell(NotificationCenterStoryCell.Key, indexPath);

            }
            else
            {
                cell = (NotificationCenterCell)tableView.DequeueReusableCell(NotificationCenterCell.Key, indexPath);
            }

            cell.Notification = dto;
            var uiCell = (UITableViewCell)cell;

            if (dto.Handled)
            {
                uiCell.BackgroundColor = UIColor.White;
            }
            else
            {
                uiCell.BackgroundColor = UIColor.FromRGB(237, 242, 250);
            }

            return uiCell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
            var dto = notificationsList[indexPath.Row];
            businessLogic.HandleNotification(dto);
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);


        partial void CloseButton_Activated(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }

        public void ReloadNotification(NotificationDTO notification)
        {
            var index = notificationsList.IndexOf(notification);
            InvokeOnMainThread(() => TableView.ReloadRows(new[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.None));
        }
    }
}