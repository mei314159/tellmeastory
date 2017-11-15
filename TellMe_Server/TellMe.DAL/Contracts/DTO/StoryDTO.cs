using System;
using System.Collections.Generic;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class StoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public bool Liked { get; set; }
        
        public List<StoryReceiverDTO> Receivers { get; set; }
    }
}