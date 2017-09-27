namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("PushTokenData")]
    public class PushTokenDataDTO
    {
        [SQLite.PrimaryKey]
        public int UserId { get; set; }

        public string Token { get; set; }

		public string PreviousToken { get; set; }

        public bool SentToServer { get; set; }
    }
}