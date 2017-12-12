namespace TellMe.Web.DAL.Types.Domain
{
    public class StoryReceiver : EntityBase<int>
    {
        public int StoryId { get; set; }
        public string UserId { get; set; }
        public int? TribeId { get; set; }
        
        public virtual Tribe Tribe { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Story Story { get; set; }
    }
}