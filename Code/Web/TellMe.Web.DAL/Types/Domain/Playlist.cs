using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.Types.Domain
{
    public class Playlist : EntityBase<int>
    {
        public string Name { get; set; }
        public string UserId { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<PlaylistStory> Stories { get; set; }
    }
}