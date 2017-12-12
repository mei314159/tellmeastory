using TellMe.Shared.Contracts.Enums;

namespace TellMe.Shared.Contracts.DTO
{
    public class SharedContactDTO
    {
        public ContactType Type { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        
        public int? TribeId { get; set; }

        public SharedStorytellerDTO User { get; set; }

        public SharedTribeDTO Tribe { get; set; }
    }
}
