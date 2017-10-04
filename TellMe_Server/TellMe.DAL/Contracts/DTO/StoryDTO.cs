using System;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class StoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? RequestDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public DateTime? CreateDateUtc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public StoryStatus Status { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
