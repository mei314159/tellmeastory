using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface IStoryService : IService
    {
        Task<ICollection<StoryDTO>> RequestStoryAsync(string requestSenderId, StoryRequestDTO dto);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId);
        Task<ICollection<StoryDTO>> SendStoryAsync(string senderId, SendStoryDTO dto);
    }
}