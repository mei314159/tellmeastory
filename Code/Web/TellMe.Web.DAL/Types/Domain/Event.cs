using System;
using System.Collections.Generic;

namespace TellMe.DAL.Types.Domain
{
    public class Event : EntityBase<int>
    {
        public string HostId { get; set; }

        public DateTime DateUtc { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool ShareStories { get; set; }

        public virtual ApplicationUser Host { get; set; }

        public virtual ICollection<StoryRequest> StoryRequests { get; set; }

        public virtual ICollection<EventAttendee> Attendees { get; set; }
    }
}