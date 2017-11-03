using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IStoryService : IService
    {
        Task<ICollection<StoryRequestDTO>> RequestStoryAsync(string requestSenderId, IEnumerable<StoryRequestDTO> requests);
        Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int skip);
        Task<ICollection<StoryDTO>> GetAllAsync(string userId, string currentUserId, int skip);
        Task<StoryStatus> RejectRequestAsync(string currentUserId, int requestId);
    }
}