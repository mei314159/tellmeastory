using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.DTO
{
    public class StoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public int? EventId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public bool Liked { get; set; }
        
        public List<StoryReceiverDTO> Receivers { get; set; }
    }
}