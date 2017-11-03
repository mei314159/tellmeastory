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

        Task<StorytellerDTO> GetStorytellerAsync(string currentUserId, string userId);

        Task<bool> AddTokenAsync(RefreshToken token);

        Task<bool> ExpireTokenAsync(RefreshToken token);

        Task<RefreshToken> GetTokenAsync(string token, string clientId);

        Task<IReadOnlyCollection<ContactDTO>> SearchContactsAsync(string currentUserId, string fragment, ContactsMode mode, int? skip = null);

        Task<FriendshipStatus> AddToFriendsAsync(string currentUserId, string userId);
        Task<FriendshipStatus> RejectFriendshipRequestAsync(string currentUserId, string userId);
    }
}