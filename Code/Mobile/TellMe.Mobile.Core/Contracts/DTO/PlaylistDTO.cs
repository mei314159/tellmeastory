using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

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

        [TextBlob("StoryIdsBlobbed")]
        public ICollection<int> StoryIds { get; set; }

        public string StoryIdsBlobbed { get; set; }
    }
}