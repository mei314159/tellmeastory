using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Shared.Contracts.DTO;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface IStoryService : IService
    {
        Task<ICollection<StoryRequestDTO>> RequestStoryAsync(string requestSenderId, RequestStoryDTO request);
        Task<StoryDTO> SendStoryAsync(string senderId, SendStoryDTO dto);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(string currentUserId, int eventId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(int tribeId, string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryDTO>> GetAllAsync(string userId, string currentUserId, DateTime olderThanUtc);
        Task<ICollection<StoryListDTO>> SearchAsync(string currentUserId, string fragment, int skip);
        Task<ICollection<StoryReceiverDTO>> GetStoryReceiversAsync(string currentUserId, int storyId);
        Task<StoryStatus> RejectRequestAsync(string currentUserId, int requestId);
        Task<bool> LikeAsync(string currentUserId, int storyId);
        Task<bool> DislikeAsync(string userId, int storyId);
        Task AddToPlaylistAsync(string currentUserId, int storyId, int playlistId);
        Task FlagAsObjectionableAsync(string currentUserId, int storyId);
        Task UnflagAsObjectionableAsync(string currentUserId, int storyId);
        Task RemoveStoryAsync(string userId, int storyId, string confirmationToken);
    }
}