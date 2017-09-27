namespace TellMe.Core.Contracts.DTO
{
    public class StoryRequestDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ReceiverId { get; set; }
    }
}