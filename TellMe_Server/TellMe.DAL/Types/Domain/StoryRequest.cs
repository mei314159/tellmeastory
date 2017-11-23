using System;
using System.Collections.Generic;

namespace TellMe.DAL.Types.Domain
{
    public class StoryRequest : EntityBase<int>
    {
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string SenderId { get; set; }
        public string UserId { get; set; }
        public int? TribeId { get; set; }
        public int? EventId { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        public virtual Event Event { get; set; }
        public virtual Tribe Tribe { get; set; }
        public virtual ICollection<Story> Stories { get; set; }

        public virtual ICollection<StoryRequestStatus> Statuses { get; set; }
    }
}