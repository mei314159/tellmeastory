using System.Collections.Generic;

namespace TellMe.Shared.Contracts.DTO
{
    public class SharePlaylistDTO
    {
        public int PlaylistId { get; set; }
        public ICollection<string> UserIds { get; set; }
    }
}