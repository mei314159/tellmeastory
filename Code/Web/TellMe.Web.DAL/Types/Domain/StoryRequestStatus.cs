namespace TellMe.DAL.Types.Domain
{
    public class StoryRequestStatus : EntityBase<int>
    {
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public int? TribeId { get; set; }

        public StoryStatus Status { get; set; }
        
        public virtual StoryRequest Request { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Tribe Tribe { get; set; }
    }
}