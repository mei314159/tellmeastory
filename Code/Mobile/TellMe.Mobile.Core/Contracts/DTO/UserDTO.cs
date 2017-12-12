namespace TellMe.Mobile.Core.Contracts.DTO
{
    public class UserDTO
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
    }
}