using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TellMe.iOS.Views;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Core
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly INotificationHandlerBusinessLogic _businessLogic;
        private readonly IRemotePlaylistsDataService _remotePlaylistsDataService;

        public NotificationHandler(INotificationHandlerBusinessLogic businessLogic, IRemotePlaylistsDataService remotePlaylistsDataService)
        {
            _businessLogic = businessLogic;
            _remotePlaylistsDataService = remotePlaylistsDataService;
        }

        public async Task<bool?> ProcessNotificationAsync(NotificationDTO notification, IView view)
        {
            bool? result;
            Overlay overlay;
            switch (notification.Type)
            {
                case NotificationTypeEnum.StoryRequest:
                {
                    var promise = new TaskCompletionSource<bool?>();

                    if (notification.Handled)
                        return null;

                    var extra = ((JObject) notification.Extra).ToObject<StoryRequestDTO>();
                    var alert = UIAlertController
                        .Create("Story request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive,
                        x => RejectStoryRequestTouched(notification.Id, extra, promise)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default,
                        x => AcceptStoryRequestTouched(notification.Id, extra, view, promise)));
                    ((UIViewController) view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);

                    break;
                }
                case NotificationTypeEnum.FriendshipRequest:
                {
                    if (notification.Handled)
                        return null;
                    var promise = new TaskCompletionSource<bool?>();
                    var extra = ((JObject) notification.Extra).ToObject<StorytellerDTO>();
                    var alert = UIAlertController
                        .Create("Friendship request",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive,
                        x => RejectFriendshipTouched(notification.Id, extra, promise)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default,
                        x => AcceptFriendshipTouched(notification.Id, extra, promise)));

                    ((UIViewController) view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);
                    break;
                }
                case NotificationTypeEnum.TribeInvite:
                {
                    if (notification.Handled)
                        return null;
                    var promise = new TaskCompletionSource<bool?>();
                    var extra = ((JObject) notification.Extra).ToObject<TribeDTO>();
                    var alert = UIAlertController
                        .Create("Join a Tribe",
                            notification.Text,
                            UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    alert.AddAction(UIAlertAction.Create("Reject", UIAlertActionStyle.Destructive,
                        x => RejectTribeInvitationTouched(notification.Id, extra, promise)));
                    alert.AddAction(UIAlertAction.Create("Accept", UIAlertActionStyle.Default,
                        x => AcceptTribeInvitationTouched(notification.Id, extra, promise)));
                    ((UIViewController) view).PresentViewController(alert, true, null);
                    result = await promise.Task.ConfigureAwait(false);
                    break;
                }
                case NotificationTypeEnum.Story:
                {
                    var extra = ((JObject) notification.Extra).ToObject<StoryDTO>();
                    overlay = new Overlay("Wait");
                    overlay.PopUp();
                    result = await _businessLogic.NavigateStory(notification.Id, extra, view).ConfigureAwait(false);
                    overlay.Close();
                    break;
                }
                case NotificationTypeEnum.SharePlaylist:
                    {
                        var extra = ((JObject)notification.Extra).ToObject<PlaylistDTO>();
                        overlay = new Overlay("Wait");
                        overlay.PopUp();

                        var playlist = await _remotePlaylistsDataService.GetAsync(extra.Id).ConfigureAwait(false);
                        result = await _businessLogic.NavigatePlaylist(notification.Id, playlist.IsSuccess ? playlist.Data ?? extra : extra, view).ConfigureAwait(false);
                        overlay.Close();
                        break;
                    }
                case NotificationTypeEnum.Event:
                    var @event = ((JObject) notification.Extra).ToObject<EventDTO>();
                    overlay = new Overlay("Wait");
                    overlay.PopUp();
                    result = await _businessLogic.NavigateEvent(notification.Id, @event, view).ConfigureAwait(false);
                    overlay.Close();
                    break;
                default:
                    if (notification.Handled)
                        return null;
                    result = await _businessLogic.HandleNotification(notification.Id).ConfigureAwait(false);
                    break;
            }

            return result;
        }

        private async void RejectFriendshipTouched(int notificationId, StorytellerDTO dto,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void AcceptFriendshipTouched(int notificationId, StorytellerDTO dto,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptFriendshipRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void RejectStoryRequestTouched(int notificationId, StoryRequestDTO dto,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectStoryRequestRequestAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }

        private async void AcceptTribeInvitationTouched(int notificationId, TribeDTO dto,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);
        }

        private async void RejectTribeInvitationTouched(int notificationId, TribeDTO dto,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.RejectTribeInvitationAsync(notificationId, dto).ConfigureAwait(false);
            overlay.Close();

            promise.SetResult(result);
        }

        private async void AcceptStoryRequestTouched(int notificationId, StoryRequestDTO dto, IView view,
            TaskCompletionSource<bool?> promise)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var result = await _businessLogic.AcceptStoryRequestRequest(notificationId, dto, view)
                .ConfigureAwait(false);
            overlay.Close();
            promise.SetResult(result);
        }
    }
}