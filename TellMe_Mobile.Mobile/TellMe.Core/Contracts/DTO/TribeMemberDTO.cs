namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("TribeMembers")]
    public class TribeMemberDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public int TribeId { get; set; }

        public string TribeName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string UserPictureUrl { get; set; }

        [SQLiteNetExtensions.Attributes.TextBlob("StatusBlobbed")]
        public TribeMemberStatus Status { get; set; }

        public string StatusBlobbed { get; set; }
    }
}