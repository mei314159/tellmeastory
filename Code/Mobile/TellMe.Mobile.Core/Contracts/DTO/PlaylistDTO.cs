using System;
using System.Collections.Generic;
using SQLite;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DTO
{
    [Table("Events")]
    public class PlaylistDTO
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public string AuthorId { get; set; }

        public string AuthorUserName { get; set; }

        public string AuthorPictureUrl { get; set; }

        public DateTime CreateDateUtc { get; set; }
        
        public int StoriesCount { get; set; }

        [Ignore]
        public List<StoryOrderDTO> Stories { get; set; }

        protected bool Equals(PlaylistDTO other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlaylistDTO) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}