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

        public DateTime CreateDateUtc { get; set; }

        public string UserId { get; set; }

        [SQLite.Ignore]
        public List<StoryListDTO> Stories { get; set; }
    }
}