using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.Types.Domain
{
    public class Story : EntityBase<int>
    {
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public int? RequestId { get; set; }
        public int? EventId { get; set; }
        public string SenderId { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }

        public virtual StoryRequest Request { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual Event Event { get; set; }

        public virtual ICollection<StoryReceiver> Receivers { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<StoryLike> Likes { get; set; }
        
        public virtual ICollection<PlaylistStory> Playlists { get; set; }
        
        public virtual ICollection<ObjectionableStory> ObjectionableStories { get; set; }
    }
}