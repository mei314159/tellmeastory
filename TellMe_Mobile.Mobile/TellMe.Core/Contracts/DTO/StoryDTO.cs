using System;
using System.Collections.Generic;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Stories")]
    public class StoryDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }

        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }

        public List<StoryReceiverDTO> Receivers { get; set; }
    }
}