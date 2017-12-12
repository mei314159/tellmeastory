using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class NotificationCenterController : UIViewController, IUITableViewDelegate, IUITableViewDataSource,
        INotificationsCenterView
    {
        private readonly List<NotificationDTO> _notificationsList = new List<NotificationDTO>();

        private INotificationCenterBusinessLogic _businessLogic;

        public NotificationCenterController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            _businessLogic = IoC.GetInstance<INotificationCenterBusinessLogic>();
            _businessLogic.View = this;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DataSource = this;
            this.TableView.Delegate = this;
            this.TableView.RegisterNibForCellReuse(NotificationCenterCell.Nib, NotificationCenterCell.Key);
            this.TableView.RegisterNibForCellReuse(NotificationCenterStoryCell.Nib, NotificationCenterStoryCell.Key);
            this.TableView.RefreshControl = new UIRefreshControl();
            this.TableView.RefreshControl.ValueChanged += (s, e) => LoadNotificationsAsync(true);
            Task.Run(() => LoadNotificationsAsync(false));
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetNavigationBarHidden(true, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            this.NavigationController.SetNavigationBarHidden(false, true);
        }

        private async Task LoadNotificationsAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadNotificationsAsync(forceRefresh).ConfigureAwait(false);
            InvokeOnMainThread(() => TableView.RefreshControl.EndRefreshing());
        }

        public void DisplayNotifications(IReadOnlyCollection<NotificationDTO> notifications)
        {
            lock (((ICollection) _notificationsList).SyncRoot)
            {
                _notificationsList.Clear();
                _notificationsList.AddRange(notifications);
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _notificationsList.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var dto = _notificationsList[indexPath.Row];
            INotificationCenterCell cell;
            if (dto.Type == NotificationTypeEnum.Story)
            {
                cell = (NotificationCenterStoryCell) tableView.DequeueReusableCell(NotificationCenterStoryCell.Key,
                    indexPath);
            }
            else
            {
                cell = (NotificationCenterCell) tableView.DequeueReusableCell(NotificationCenterCell.Key, indexPath);
            }

            cell.Notification = dto;
            var uiCell = (UITableViewCell) cell;

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
            var dto = _notificationsList[indexPath.Row];
            _businessLogic.HandleNotification(dto);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);


        partial void CloseButton_Activated(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }

        public void ReloadNotification(NotificationDTO notification)
        {
            var index = _notificationsList.IndexOf(notification);
            InvokeOnMainThread(() =>
                TableView.ReloadRows(new[] {NSIndexPath.FromRowSection(index, 0)}, UITableViewRowAnimation.None));
        }
    }
}