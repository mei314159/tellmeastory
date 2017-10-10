namespace TellMe.Core.Contracts.DTO
{
    public class SendStoryDTO
    {
        public int? Id { get; set; }

        public string ReceiverId { get; set; }

        public string Title { get; set; }

        public string[] ReceiverIds { get; set; }

        public string VideoUrl { get; set; }

        public string PreviewUrl { get; set; }
    }
}