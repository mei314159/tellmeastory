using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.DTO
{
    public class PlaylistDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string UserId { get; set; }

        public ICollection<StoryDTO> Stories { get; set; }
    }
}