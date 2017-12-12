using System;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.DTO
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public string RecipientId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public bool Handled { get; set; }
        
        public NotificationTypeEnum Type { get; set; }

        public object Extra { get; set; }
    }
}
