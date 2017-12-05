using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class PlaylistStory : IEntityBase
    {
        public int PlaylistId { get; set; }

        public int StoryId { get; set; }

        public virtual Playlist Playlist { get; set; }

        public virtual Story Story { get; set; }
    }
}