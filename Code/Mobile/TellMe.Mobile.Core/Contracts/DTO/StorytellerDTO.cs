﻿namespace TellMe.Mobile.Core.Contracts.DTO
{
    [SQLite.Table("Storytellers")]
    public class StorytellerDTO
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }

        public int FriendsCount { get; set; }
        public int EventsCount { get; set; }
        public int StoriesCount { get; set; }
        
        [SQLiteNetExtensions.Attributes.TextBlob("StatusBlobbed")]
        public FriendshipStatus FriendshipStatus { get; set; }

        public string StatusBlobbed { get; set; }
    }
}