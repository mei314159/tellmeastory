﻿using System;
using System.Collections.Generic;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Stories")]
    public class StoryDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }

        public bool Liked { get; set; }

        [SQLiteNetExtensions.Attributes.OneToMany("StoryId", CascadeOperations =
            SQLiteNetExtensions.Attributes.CascadeOperation.All)]
        public List<StoryReceiverDTO> Receivers { get; set; }
    }
}