using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Shared.Contracts.DTO;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts.DTO;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface IUserService : IService
    {
        Task SendRegistrationConfirmationEmailAsync(string userId, string email, string confirmationToken);
        
        Task SendRequestToJoinAsync(string email, string senderFullName);
        
        Task<ApplicationUser> GetAsync(string id);

        Task<SharedStorytellerDTO> GetStorytellerAsync(string currentUserId, string userId);

        Task<bool> AddTokenAsync(RefreshToken token);

        Task<bool> ExpireTokenAsync(RefreshToken token);

        Task<RefreshToken> GetTokenAsync(string token, string clientId);

        Task<IReadOnlyCollection<SharedContactDTO>> SearchContactsAsync(string currentUserId, string fragment, ContactsMode mode, int skip);

        Task<FriendshipStatus> AddToFriendsAsync(string currentUserId, string userId);
        Task<FriendshipStatus> RejectFriendshipRequestAsync(string currentUserId, string userId);
    }
}