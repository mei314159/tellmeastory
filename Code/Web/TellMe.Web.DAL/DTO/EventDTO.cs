using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.DTO
{
    public class EventDTO
    {
        public int Id { get; set; }

        public string HostId { get; set; }

        public string HostPictureUrl { get; set; }

        public string HostUserName { get; set; }

        public DateTime DateUtc { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public bool ShareStories { get; set; }
        
        public List<EventAttendeeDTO> Attendees { get; set; }
    }
}