namespace TellMe.Web.DAL.Types.Domain
{
    public class TribeMember : EntityBase<int>
    {
        public string UserId { get; set; }

        public int TribeId { get; set; }

        public TribeMemberStatus Status { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Tribe Tribe { get; set; }
    }
}