using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemoteStorytellersDataService : IRemoteDataService
    {
        Task<Result<List<ContactDTO>>> SearchAsync(string fragment, int skip, ContactsMode mode);
        Task<Result<FriendshipStatus>> SendFriendshipRequestAsync(string id);
        Task<Result<FriendshipStatus>> RejectFriendshipRequestAsync(string id, int notificationId);
        Task<Result<FriendshipStatus>> AcceptFriendshipRequestAsync(string id, int notificationId);
        Task<Result> SendRequestToJoinAsync(string email);
        Task<Result<StorytellerDTO>> GetByIdAsync(string storytellerId);
    }
}