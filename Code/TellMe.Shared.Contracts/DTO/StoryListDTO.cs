using System;

namespace TellMe.Shared.Contracts.DTO
{
    public class StoryListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PreviewUrl { get; set; }
        public string VideoUrl { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public int Order { get; set; }
    }
}