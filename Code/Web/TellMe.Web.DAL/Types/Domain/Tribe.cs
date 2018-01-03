using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.Types.Domain
{
    public class Tribe : EntityBase<int>
    {
        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<TribeMember> Members { get; set; }
        
        public virtual ICollection<EventAttendee> Events { get; set; }
        
        public virtual ICollection<StoryReceiver> Stories { get; set; }
    }
}