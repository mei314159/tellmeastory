namespace TellMe.Core.Contracts.DTO
{
    public class SendStoryDTO
    {
        public string Title { get; set; }

        public int? RequestId { get; set; }

        public string[] ReceiverIds { get; set; }

        public string VideoUrl { get; set; }

        public string PreviewUrl { get; set; }
    }
}