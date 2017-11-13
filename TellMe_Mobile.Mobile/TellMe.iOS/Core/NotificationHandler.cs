using System.Threading.Tasks;
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
        private readonly IView _view;
        private readonly NotificationHandlerBusinessLogic businessLogic;

        public NotificationHandler(IRouter router, IView view)
        {
            _view = view;
            businessLogic = new NotificationHandlerBusinessLogic(
                router,
                new RemoteStoriesDataService(),
                new RemoteStorytellersDataService(),
                new RemoteNotificationsDataService(),
                new RemoteTribesDataService());
        }

        public async Task<bool?> ProcessNotification(NotificationDTO notification)
        {
            bool? result;
            if (notification.Type == NotificationTypeEnum.StoryRequest)
            {
                var promise = new TaskCompletionSource<bool?>();

                if (notification.Handled)
                    return null;

                var extra = ((JObject)notification.Extra).ToObject<StoryRequestDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Story request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectStoryRequestTouched(notification.Id, extra, promise)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptStoryRequestTouched(notification.Id, extra, _view, promise)));
                ((UIViewController)_view).PresentViewController(alert, true, null);
                result = await promise.Task.ConfigureAwait(false);

            }
            else if (notification.Type == NotificationTypeEnum.FriendshipRequest)
            {
                if (notification.Handled)
                    return null;
                var promise = new TaskCompletionSource<bool?>();
                var extra = ((JObject)notification.Extra).ToObject<StorytellerDTO>();
                var alert = UIAlertController
                    .Create("Friendship request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectFriendshipTouched(notification.Id, extra, promise)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptFriendshipTouched(notification.Id, extra, promise)));

                ((UIViewController)_view).PresentViewController(alert, true, null);
                result = await promise.Task.ConfigureAwait(false);
            }
            else if (notification.Type == NotificationTypeEnum.TribeInvite)
            {
                if (notification.Handled)
                    return null;
                var promise = new TaskCompletionSource<bool?>();
                var extra = ((JObject)notification.Extra).ToObject<TribeDTO>();
                UIAlertController alert = UIAlertController
                    .Create("Join a Tribe",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectTribeInvitationTouched(notification.Id, extra, promise)));
                alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptTribeInvitationTouched(notification.Id, extra, promise)));
                ((UIViewController)_view).PresentViewController(alert, true, null);
                result = await promise.Task.ConfigureAwait(false);
            }
            else if (notification.Type == NotificationTypeEnum.Story)
            {
                var extra = ((JObject)notification.Extra).ToObject<StoryDTO>();
                var overlay = new Overlay("Wait");
                overlay.PopUp(true);
                result = await businessLogic.ViewStory(notification.Id, extra, _view).ConfigureAwait(false);
                overlay.Close();
            }
            else
            {
                if (notification.Handled)
                    return null;
                result = await businessLogic.HandleNotification(notification.Id).ConfigureAwait(false);
            }

            return result;
        }

        async void RejectFriendshipTouched(int notificationId, StorytellerDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.RejectFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        async void AcceptFriendshipTouched(int notificationId, StorytellerDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.AcceptFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        async void RejectStoryRequestTouched(int notificationId, StoryRequestDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.RejectStoryRequestRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        async void AcceptTribeInvitationTouched(int notificationId, TribeDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.AcceptTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);
        }

        async void RejectTribeInvitationTouched(int notificationId, TribeDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.RejectTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);

        }

        async void AcceptStoryRequestTouched(int notificationId, StoryRequestDTO dto, IView view, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            var result = await businessLogic.AcceptStoryRequestRequest(notificationId, dto, view).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }
    }
}
