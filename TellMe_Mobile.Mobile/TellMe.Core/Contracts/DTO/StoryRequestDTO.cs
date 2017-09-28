namespace TellMe.Core.Contracts.DTO
{
    public class StoryRequestDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ReceiverId { get; set; }
    }
}