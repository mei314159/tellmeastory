using System;
using TellMe.Web.DAL.Contracts.Domain;

namespace TellMe.Web.DAL.Types.Domain
{
    public class ObjectionableStory : IEntityBase
    {
        public int StoryId { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; }
        
        public virtual ApplicationUser User { get; set; }

        public virtual Story Story { get; set; }
    }
}