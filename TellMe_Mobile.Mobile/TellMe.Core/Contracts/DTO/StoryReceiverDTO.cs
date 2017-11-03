namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("StoryReceivers")]
    public class StoryReceiverDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public int StoryId { get; set; }

        public string UserId { get; set; }

        public int? TribeId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPictureUrl { get; set; }
    }
}