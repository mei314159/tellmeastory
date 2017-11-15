using System;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Notifications")]
    public class NotificationDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public string RecipientId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        [SQLiteNetExtensions.Attributes.TextBlob("TypeBlob")]
        public NotificationTypeEnum Type { get; set; }

		public bool Handled { get; set; }

        [SQLiteNetExtensions.Attributes.TextBlob("ExtraBlob")]
        public object Extra { get; set; }

        public string ExtraBlob { get; set; }

        public string TypeBlob { get; set; }
    }
}