using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Domain;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IUserService : IService
    {
        Task<ApplicationUser> GetAsync(string id);

        Task<bool> AddTokenAsync(RefreshToken token);

        Task<bool> ExpireTokenAsync(RefreshToken token);

        Task<RefreshToken> GetTokenAsync(string token, string clientId);
        
        Task<IReadOnlyCollection<StorytellerDTO>> SearchAsync(string currentUserId, string fragment);
        Task<IReadOnlyCollection<StorytellerDTO>> GetAllFriendsAsync(string currentUserId);
        Task<FriendshipStatus> AddToFriendsAsync(string currentUserId, string userId);
    }
}