using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class ApplicationUser : IdentityUser, IEntityBase<string>
    {
        public string FullName { get; set; }

        public string PictureUrl { get; set; }

        public virtual ICollection<Friendship> Friends { get; set; }

        public virtual ICollection<PushNotificationClient> PushNotificationClients { get; set; }

        public virtual ICollection<Story> SentStories { get; set; }

        public virtual ICollection<TribeMember> Tribes { get; set; }
        
        public virtual ICollection<StoryLike> LikedStories { get; set; }
    }
}
