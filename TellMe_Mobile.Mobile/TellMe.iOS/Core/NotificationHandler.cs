using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Views;
using UIKit;

namespace TellMe.iOS.Core
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly INotificationHandlerBusinessLogic _businessLogic;

        public NotificationHandler(INotificationHandlerBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
        }

        public async Task<bool?> ProcessNotificationAsync(NotificationDTO notification, IView view)
        {
            bool? result;
            switch (notification.Type)
            {
                case NotificationTypeEnum.StoryRequest:
                {
                    var promise = new TaskCompletionSource<bool?>();

                    if (notification.Handled)
                        return null;

                    var extra = ((JObject)notification.Extra).ToObject<StoryRequestDTO>();
                    var alert = UIAlertController
                        .Create("Story request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectStoryRequestTouched(notification.Id, extra, promise)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptStoryRequestTouched(notification.Id, extra, view, promise)));
                    ((UIViewController)view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);

                    break;
                }
                case NotificationTypeEnum.FriendshipRequest:
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

                    ((UIViewController)view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);
                    break;
                }
                case NotificationTypeEnum.TribeInvite:
                {
                    if (notification.Handled)
                        return null;
                    var promise = new TaskCompletionSource<bool?>();
                    var extra = ((JObject)notification.Extra).ToObject<TribeDTO>();
                    var alert = UIAlertController
                        .Create("Join a Tribe",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive, x => RejectTribeInvitationTouched(notification.Id, extra, promise)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default, x => AcceptTribeInvitationTouched(notification.Id, extra, promise)));
                    ((UIViewController)view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);
                    break;
                }
                case NotificationTypeEnum.Story:
                {
                    var extra = ((JObject)notification.Extra).ToObject<StoryDTO>();
                    var overlay = new Overlay("Wait");
                    overlay.PopUp();
                    result = await _businessLogic.ViewStory(notification.Id, extra, view).ConfigureAwait(false);
                    overlay.Close();
                    break;
                }
                default:
                    if (notification.Handled)
                        return null;
                    result = await _businessLogic.HandleNotification(notification.Id).ConfigureAwait(false);
                    break;
            }

            return result;
        }

        private async void RejectFriendshipTouched(int notificationId, StorytellerDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void AcceptFriendshipTouched(int notificationId, StorytellerDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void RejectStoryRequestTouched(int notificationId, StoryRequestDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectStoryRequestRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void AcceptTribeInvitationTouched(int notificationId, TribeDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);
        }

        private async void RejectTribeInvitationTouched(int notificationId, TribeDTO dto, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);

        }

        private async void AcceptStoryRequestTouched(int notificationId, StoryRequestDTO dto, IView view, TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptStoryRequestRequest(notificationId, dto, view).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }
    }
}
