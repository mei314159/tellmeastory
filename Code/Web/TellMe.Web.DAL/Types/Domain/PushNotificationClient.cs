using TellMe.Web.DAL.Contracts.Domain;

namespace TellMe.Web.DAL.Types.Domain
{
    public class PushNotificationClient : IEntityBase<int>
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public OsType OsType { get; set; }
		public string AppVersion { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}