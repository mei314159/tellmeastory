namespace TellMe.DAL.Types.Domain
{
    public class Contact : EntityBase<int>
    {
        public string PhoneNumber { get; set; }
        public long PhoneNumberDigits { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}