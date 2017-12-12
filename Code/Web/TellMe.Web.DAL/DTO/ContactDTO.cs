using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.DTO
{
    public class ContactDTO
    {
        public ContactType Type { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        
        public int? TribeId { get; set; }

        public StorytellerDTO User { get; set; }

        public TribeDTO Tribe { get; set; }
    }
}
