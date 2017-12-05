using System.Collections.Generic;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.Web.DTO
{
    public class RequestStoryDTO
    {
        public List<StoryRequestDTO> Requests { get; set; }
    }
}
