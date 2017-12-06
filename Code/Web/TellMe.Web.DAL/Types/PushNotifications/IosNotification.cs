using Newtonsoft.Json;

namespace TellMe.Web.DAL.Types.PushNotifications
{
    public class IosNotification<T>
    {
        [JsonProperty(PropertyName = "aps")]
        public IosNotificationAps Data { get; set; }

        [JsonProperty(PropertyName = "notificationType")]
        public NotificationTypeEnum NotificationType { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public T Extra { get; set; }

        [JsonProperty(PropertyName = "notificationId")]
        public int? NotificationId { get; set; }
    }
}
