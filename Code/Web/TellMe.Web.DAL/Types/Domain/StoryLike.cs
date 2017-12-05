using System.ComponentModel.DataAnnotations.Schema;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class StoryLike : IEntityBase
    {
        [Column("UserId")]
        public string UserId { get; set; }
        
        [Column("StoryId")]
        public int StoryId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Story Story { get; set; }
    }
}