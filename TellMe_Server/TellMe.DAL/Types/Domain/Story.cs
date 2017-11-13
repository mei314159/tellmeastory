using System;
using System.Collections.Generic;

namespace TellMe.DAL.Types.Domain
{
    public class Story : EntityBase<int>
    {
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public int? RequestId { get; set; }
        public string SenderId { get; set; }

        public virtual StoryRequest Request { get; set; }
        public virtual ApplicationUser Sender { get; set; }

        public virtual ICollection<StoryReceiver> Receivers { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public int CommentsCount { get; set; }
    }
}