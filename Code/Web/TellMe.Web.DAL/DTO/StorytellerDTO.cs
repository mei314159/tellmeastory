using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.DTO
{
    public class StorytellerDTO
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public FriendshipStatus FriendshipStatus { get; set; }
    }
}
