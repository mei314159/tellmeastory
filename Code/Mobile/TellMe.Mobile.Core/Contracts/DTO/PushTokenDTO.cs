namespace TellMe.Mobile.Core.Contracts.DTO
{
    public class PushTokenDTO
    {
        public string Token { get; set; }
        public string OldToken { get; set; }
        public OsType OsType { get; set; }
        public string AppVersion { get; set; }
    }
}