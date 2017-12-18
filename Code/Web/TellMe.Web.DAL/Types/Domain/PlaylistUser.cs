using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts.Domain;

namespace TellMe.Web.DAL.Types.Domain
{
    public class PlaylistUser : IEntityBase
    {
        public int PlaylistId { get; set; }
        
        public string UserId { get; set; }
        
        public PlaylistUserType Type { get; set; }
        
        public virtual Playlist Playlist { get; set; }
        
        public virtual ApplicationUser User { get; set; }
    }
}