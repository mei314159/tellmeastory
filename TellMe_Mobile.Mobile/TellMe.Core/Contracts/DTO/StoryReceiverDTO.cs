namespace TellMe.Core.Contracts.DTO
{
    public class StoryReceiverDTO
    {
        public string UserId { get; set; }
        public int? TribeId { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverPictureUrl { get; set; }
    }
}