using SQLite;

namespace TellMe.Core.Contracts.DTO
{
    [Table("EventAttendees")]
    public class EventAttendeeDTO
    {
        [PrimaryKey]
        public int Id { get; set; }
        
        public int EventId { get; set; }

        public string UserId { get; set; }

        public int? TribeId { get; set; }
        
        public EventAttendeeStatus Status { get; set; }

        public string AttendeeName { get; set; }
        
        public string AttendeeFullName { get; set; }
        
        public string AttendeePictureUrl { get; set; }
    }
}