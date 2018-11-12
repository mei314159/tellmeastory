using System;
using System.Collections.Generic;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Web.DAL.DTO
{
    public class PlaylistDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string AuthorId { get; set; }
        
        public string AuthorUserName { get; set; }

        public string AuthorPictureUrl { get; set; }

        public DateTime CreateDateUtc { get; set; }
        
        public int StoriesCount { get; set; }

        public ICollection<StoryOrderDTO> Stories { get; set; }
    }
}