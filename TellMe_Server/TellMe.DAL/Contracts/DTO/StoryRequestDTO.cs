using System;

namespace TellMe.DAL.Contracts.DTO
{
    public class StoryRequestDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string SenderName { get; set; }
        public string SenderPictureUrl { get; set; }
        public string ReceiverName { get; set; }
        public string SenderId { get; set; }
        public string UserId { get; set; }
        public int? TribeId { get; set; }
        public int? EventId { get; set; }
    }
}
