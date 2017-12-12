using System.Collections.Generic;

namespace TellMe.Shared.Contracts.DTO
{
    public class RequestStoryDTO
    {
        public string Title { get; set; }
        public int? EventId { get; set; }
        public ICollection<SharedContactDTO> Recipients { get; set; }
    }
}