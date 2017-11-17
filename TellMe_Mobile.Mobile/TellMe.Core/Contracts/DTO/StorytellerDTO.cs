namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Storytellers")]
    public class StorytellerDTO
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }

        [SQLiteNetExtensions.Attributes.TextBlob("StatusBlobbed")]
        public FriendshipStatus FriendshipStatus { get; set; }

        public string StatusBlobbed { get; set; }
    }
}