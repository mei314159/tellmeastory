namespace TellMe.DAL.Types.Domain
{
    public class Contact : EntityBase<int>
    {
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}