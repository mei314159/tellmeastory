using System;

namespace TellMe.DAL.Types.Domain
{
    public class EventAttendee : EntityBase<int>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public int? TribeId { get; set; }
        public EventAttendeeStatus Status { get; set; }
        public DateTime CreateDateUtc { get; set; }
        
        public virtual Event Event { get; set; }
        public virtual Tribe Tribe { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}