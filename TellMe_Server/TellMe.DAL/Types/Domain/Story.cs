using System;

namespace TellMe.DAL.Types.Domain
{
    public class Story : EntityBase<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public StoryStatus Status { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}