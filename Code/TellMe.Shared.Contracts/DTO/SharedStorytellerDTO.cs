using TellMe.Shared.Contracts.Enums;

namespace TellMe.Shared.Contracts.DTO
{
    public class SharedStorytellerDTO
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public virtual FriendshipStatus FriendshipStatus { get; set; }
    }
}
