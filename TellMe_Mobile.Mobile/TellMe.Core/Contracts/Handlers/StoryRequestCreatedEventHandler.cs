using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.Handlers
{
    public delegate void StoryRequestCreatedEventHandler(RequestStoryDTO dto, ICollection<ContactDTO> recipients);
}