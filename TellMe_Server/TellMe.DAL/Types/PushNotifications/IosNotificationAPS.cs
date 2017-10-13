using Newtonsoft.Json;

namespace TellMe.DAL.Types.PushNotifications
{
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
