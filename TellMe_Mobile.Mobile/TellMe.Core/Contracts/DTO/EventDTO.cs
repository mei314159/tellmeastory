using System;
using System.Collections.Generic;
using SQLite;

namespace TellMe.Core.Contracts.DTO
{
    [Table("Events")]
    public class EventDTO
    {
        [PrimaryKey]
        public int Id { get; set; }
        
        public string HostId { get; set; }
        
        public string HostPictureUrl { get; set; }
        
        public string HostUserName { get; set; }
        
        public DateTime DateUtc { get; set; }
        
        public DateTime CreateDateUtc { get; set; }
        
        public string Title { get; set; }
        
        public string Description{ get; set; }

        public List<EventAttendeeDTO> Attendees { get; set; }
    }
}