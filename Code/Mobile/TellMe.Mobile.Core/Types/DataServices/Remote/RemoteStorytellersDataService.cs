using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
{
    public class RemoteStorytellersDataService : IRemoteStorytellersDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteStorytellersDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<List<ContactDTO>>> SearchAsync(string fragment, int skip, ContactsMode mode)
        {
            var url = string.IsNullOrWhiteSpace(fragment)
                ? $"storytellers/search/{mode}/skip/{skip}"
                : $"storytellers/search/{mode}/skip/{skip}/{fragment}";
            var result = await this._apiProvider.GetAsync<List<ContactDTO>>(url).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> SendFriendshipRequestAsync(string id)
        {
            var result = await this._apiProvider.PostAsync<FriendshipStatus>($"storytellers/{id}/add-to-friends", null)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> RejectFriendshipRequestAsync(string id, int notificationId)
        {
            var result = await this._apiProvider
                .PostAsync<FriendshipStatus>($"storytellers/{id}/reject-friendship", notificationId)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> AcceptFriendshipRequestAsync(string id, int notificationId)
        {
            var result = await this._apiProvider
                .PostAsync<FriendshipStatus>($"storytellers/{id}/add-to-friends", notificationId).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> SendRequestToJoinAsync(string email)
        {
            var result = await this._apiProvider.PostAsync<object>($"storytellers/request-to-join", email)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<StorytellerDTO>> GetByIdAsync(string storytellerId)
        {
            var result = await this._apiProvider.GetAsync<StorytellerDTO>($"storytellers/{storytellerId}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result> UnfollowAsync(string id)
        {
            var result = await this._apiProvider.PostAsync<object>($"storytellers/{id}/unfollow", null).ConfigureAwait(false);
            return result;
        }
    }
}