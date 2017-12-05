using System;

namespace TellMe.DAL.Types.Domain
{
    public class Friendship : EntityBase<int>
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Friend { get; set; }
    }
}