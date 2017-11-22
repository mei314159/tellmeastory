using System;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class EventAttendeeDTO
    {
        public int Id { get; set; }
        
        public int StoryId { get; set; }

        public string UserId { get; set; }

        public int? TribeId { get; set; }
        
        public EventAttendeeStatus Status { get; set; }
        
        public DateTime CreateDateUtc { get; set; }

        public string AttendeeName { get; set; }

        public string AttendeePictureUrl { get; set; }
    }
}