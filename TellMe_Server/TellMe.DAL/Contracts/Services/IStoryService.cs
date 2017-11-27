using System;
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
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int eventId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(int tribeId, string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(string userId, string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryReceiverDTO>> GetStoryReceiversAsync(string currentUserId, int storyId);
        Task<StoryStatus> RejectRequestAsync(string currentUserId, int requestId);
        Task<bool> LikeAsync(string currentUserId, int storyId);
        Task<bool> DislikeAsync(string userId, int storyId);
    }
}