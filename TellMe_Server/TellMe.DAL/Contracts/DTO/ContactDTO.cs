namespace TellMe.DAL.Contracts.DTO
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public long PhoneNumberDigits { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}