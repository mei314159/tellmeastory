using System;
using Newtonsoft.Json;

namespace TellMe.iOS.Core.DTO
{
    public class PushNotification
	{
		[JsonProperty(PropertyName = "aps")]
		public IosNotificationAPS Data { get; set; }

		[JsonProperty(PropertyName = "notificationType")]
		public NotificationTypeEnum NotificationType { get; set; }

		[JsonProperty(PropertyName = "extra")]
        public object Extra { get; set; }
	}
}
