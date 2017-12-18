using System;
using System.Collections.Generic;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Web.DAL.DTO
{
    public class PlaylistDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public ICollection<StoryListDTO> Stories { get; set; }
    }
}