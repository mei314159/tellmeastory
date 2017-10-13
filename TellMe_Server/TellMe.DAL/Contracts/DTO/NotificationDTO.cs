using System;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Contracts.DTO
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public string RecipientId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public NotificationTypeEnum Type { get; set; }

        public object Extra { get; set; }
    }
}
