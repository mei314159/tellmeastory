using System;
using Newtonsoft.Json.Linq;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.DTO;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Views;
using UIKit;

namespace TellMe.iOS.Core
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly NotificationHandlerBusinessLogic businessLogic;

        public NotificationHandler(IRouter router)
        {
            businessLogic = new NotificationHandlerBusinessLogic(
                router,
                new RemoteStoriesDataService(),
                new RemoteStorytellersDataService(),
                new RemoteNotificationsDataService(),
                new RemoteTribesDataService());
        }

        public void ProcessNotification(NotificationDTO notification,
            IView view,
            Action<int, bool> complete = null)
        {
            if (notification.Type == NotificationTypeEnum.StoryRequest)
            {
                if (notification.Handled)
                    return;
                
                var extra = ((JObject)notification.Extra).ToObject<StoryRequestDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Story request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectStoryRequestTouched(notification.Id, extra, complete)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptStoryRequestTouched(notification.Id, extra, view, complete)));
                ((UIViewController)view).PresentViewController(alert, true, null);

            }
            else if (notification.Type == NotificationTypeEnum.FriendshipRequest)
            {
                if (notification.Handled)
                    return;
                
                var extra = ((JObject)notification.Extra).ToObject<StorytellerDTO>();
                var alert = UIAlertController
                    .Create("Friendship request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectFriendshipTouched(notification.Id, extra, complete)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptFriendshipTouched(notification.Id, extra, complete)));

                ((UIViewController)view).PresentViewController(alert, true, null);

            }
            else if (notification.Type == NotificationTypeEnum.TribeInvite)
            {
                if (notification.Handled)
                    return;
                
                var extra = ((JObject)notification.Extra).ToObject<TribeDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Join a Tribe",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectTribeInvitationTouched(notification.Id, extra, complete)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptTribeInvitationTouched(notification.Id, extra, complete)));
                ((UIViewController)view).PresentViewController(alert, true, null);
            }
            else if (notification.Type == NotificationTypeEnum.Story)
            {
                var extra = ((JObject)notification.Extra).ToObject<StoryDTO>();
                ViewStoryAsync(notification.Id, extra, view, complete);
            }
            else if (notification.Type == NotificationTypeEnum.FriendshipAccepted)
            {
            }
            else if (notification.Type == NotificationTypeEnum.FriendshipRejected)
            {
            }
            else if (notification.Type == NotificationTypeEnum.TribeAcceptInvite)
            {
            }
            else if (notification.Type == NotificationTypeEnum.TribeRejectInvite)
            {
            }
        }

        async void ViewStoryAsync(int notificationId, StoryDTO dto, IView view, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.ViewStory(notificationId, dto, view, complete).ConfigureAwait(false);
            overlay.Close();
        }

        async void RejectFriendshipTouched(int notificationId, StorytellerDTO dto, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.RejectFriendshipRequestAsync(notificationId, dto, complete);
            overlay.Close();
        }

        async void AcceptFriendshipTouched(int notificationId, StorytellerDTO dto, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.AcceptFriendshipRequestAsync(notificationId, dto, complete);
            overlay.Close();
        }

        async void RejectStoryRequestTouched(int notificationId, StoryRequestDTO dto, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.RejectStoryRequestRequestAsync(notificationId, dto, complete);
            overlay.Close();

        }

        async void AcceptTribeInvitationTouched(int notificationId, TribeDTO dto, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.AcceptTribeInvitationAsync(notificationId, dto, complete);
            overlay.Close();
        }

        async void RejectTribeInvitationTouched(int notificationId, TribeDTO dto, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.RejectTribeInvitationAsync(notificationId, dto, complete);
            overlay.Close();

        }

        async void AcceptStoryRequestTouched(int notificationId, StoryRequestDTO dto, IView view, Action<int, bool> complete)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await businessLogic.AcceptStoryRequestRequest(notificationId, dto, view, complete);
            overlay.Close();
        }
    }
}
