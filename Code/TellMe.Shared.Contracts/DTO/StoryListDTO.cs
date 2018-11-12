using System;
using TellMe.Shared.Contracts.DTO.Interfaces;

namespace TellMe.Shared.Contracts.DTO
{
    public class StoryListDTO : StoryOrderDTO, IStoryDTO
    {
        public string Title { get; set; }
        public bool Objectionable { get; set; }
        public string PreviewUrl { get; set; }
        public string VideoUrl { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public DateTime CreateDateUtc { get; set; }
    }
}