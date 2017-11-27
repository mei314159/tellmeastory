using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.Handlers
{
    public delegate void RequestStoryEventHandler(ICollection<StoryDTO> requestedStories);
}