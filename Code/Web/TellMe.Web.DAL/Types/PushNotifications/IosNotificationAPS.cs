using Newtonsoft.Json;

namespace TellMe.Web.DAL.Types.PushNotifications
{
    public class IosNotificationAps
    {
        [JsonProperty(PropertyName = "alert")]
        public string Message { get; set; }


        [JsonProperty(PropertyName = "badge")]
        public int? Badge { get; set; }


        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }
    }
}
