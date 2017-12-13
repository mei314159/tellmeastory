using System;
using TellMe.Web.DAL.Contracts.Domain;

namespace TellMe.Web.DAL.Types.Domain
{
    public class PlaylistStory : IEntityBase
    {
        public int PlaylistId { get; set; }

        public int StoryId { get; set; }
        
        public int Order { get; set; }
        
        public virtual Playlist Playlist { get; set; }

        public virtual Story Story { get; set; }
    }
}