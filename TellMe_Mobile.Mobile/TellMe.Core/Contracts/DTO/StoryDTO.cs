using System;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Stories")]
    public class StoryDTO : StoryRequestDTO
    {
        public DateTime RequestDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public StoryStatus Status { get; set; }
        public string SenderId { get; set; }
    }
}