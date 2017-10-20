using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IStoryService : IService
    {
        Task<StoryDTO> RequestStoryAsync(string requestSenderId, StoryRequestDTO dto);
        Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int? skip = null);
        Task<StoryStatus> RejectRequestAsync(string currentUserId, int storyId);
    }
}