using System;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class StoryDTO : StoryRequestDTO
    {
        public int Id { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public StoryStatus Status { get; set; }
        public string SenderId { get; set; }
    }
}
