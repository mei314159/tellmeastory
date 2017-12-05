using System.Collections.Generic;

namespace TellMe.DAL.Contracts.DTO
{
    public class SendStoryDTO
    {
        public string Title { get; set; }
        
        public int? RequestId { get; set; }

        public List<StoryReceiverDTO> Receivers { get; set; }

        public int? NotificationId { get; set; }

        public string VideoUrl { get; set; }

        public string PreviewUrl { get; set; }
    }
}
