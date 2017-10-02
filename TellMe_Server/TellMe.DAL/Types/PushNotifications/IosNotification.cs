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
    }

    public enum NotificationTypeEnum
    {
        StoryRequest = 1
    }

    public class IosNotificationAPS
    {
        [JsonProperty(PropertyName = "alert")]
        public string Message { get; set; }


        [JsonProperty(PropertyName = "badge")]
        public int? Badge { get; set; }


        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }
    }
}
