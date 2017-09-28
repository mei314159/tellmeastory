using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface IStoryService : IService
    {
        Task RequestStoryAsync(string senderId, StoryRequestDTO dto);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, string userId);
    }
}