using System;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Stories")]
    public class StoryDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime? RequestDateUtc { get; set; }

        public DateTime UpdateDateUtc { get; set; }

        public DateTime? CreateDateUtc { get; set; }

        public StoryStatus Status { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public string VideoUrl { get; set; }

        public string PreviewUrl { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public string SenderPictureUrl { get; set; }

        public string ReceiverPictureUrl { get; set; }
    }
}