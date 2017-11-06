using Newtonsoft.Json;

namespace TellMe.DAL.Types.PushNotifications
{
    public class IosNotification<T>
    {
        [JsonProperty(PropertyName = "aps")]
        public IosNotificationAPS Data { get; set; }

        [JsonProperty(PropertyName = "notificationType")]
        public NotificationTypeEnum NotificationType { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public T Extra { get; set; }

        [JsonProperty(PropertyName = "notificationId")]
        public int? NotificationId { get; set; }
    }
}
