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
using Newtonsoft.Json.Linq;
using TellMe.iOS.Views;
using TellMe.iOS.Extensions;
using TellMe.Core;

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
            businessLogic = new NotificationCenterBusinessLogic(App.Instance.Router, new RemoteStoriesDataService(), new RemoteStorytellersDataService(), new RemoteNotificationsDataService(), this);
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
            return (UITableViewCell)cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
            var dto = notificationsList[indexPath.Row];
            if (!dto.Handled && dto.Type == NotificationTypeEnum.FriendshipRequest)
            {
                var extra = ((JObject)dto.Extra).ToObject<StorytellerDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Friendship request",
                            $"Accept friendship request from '{extra.UserName}'",
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectFriendshipTouched(dto, extra)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptFriendshipTouched(dto, extra)));
                this.PresentViewController(alert, true, null);
            }
            else if (!dto.Handled && dto.Type == NotificationTypeEnum.StoryRequest)
            {
                var extra = ((JObject)dto.Extra).ToObject<StoryDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Story request",
                            $"'{extra.ReceiverName}' requested a story '{extra.Title}'",
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectStoryRequestTouched(dto, extra)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptStoryRequestTouched(dto, extra)));
                this.PresentViewController(alert, true, null);
            }
            else if (dto.Type == NotificationTypeEnum.Story)
            {
                //TODO Show story
            }
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        async void RejectFriendshipTouched(NotificationDTO notification, StorytellerDTO dto)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.RejectFriendshipRequestAsync(notification, dto);
            overlay.Close();

        }

        async void AcceptFriendshipTouched(NotificationDTO notification, StorytellerDTO dto)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.AcceptFriendshipRequestAsync(notification, dto);
            overlay.Close();
        }

        async void RejectStoryRequestTouched(NotificationDTO notification, StoryDTO dto)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.RejectStoryRequestRequestAsync(notification, dto);
            overlay.Close();

        }

        void AcceptStoryRequestTouched(NotificationDTO notification, StoryDTO dto)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            businessLogic.AcceptStoryRequestRequest(notification, dto);
            overlay.Close();
        }

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