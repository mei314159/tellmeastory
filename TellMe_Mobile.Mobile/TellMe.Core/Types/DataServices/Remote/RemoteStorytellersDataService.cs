using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteStorytellersDataService : BaseDataService
    {
        public async Task<Result<List<StorytellerDTO>>> SearchAsync(string fragment)
        {
            var result = await this.GetAsync<List<StorytellerDTO>>($"storytellers/search/{fragment}").ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<StorytellerDTO>>> GetAllAsync()
        {
            var result = await this.GetAsync<List<StorytellerDTO>>($"storytellers").ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> SendFriendshipRequestAsync(string id)
        {
            var result = await this.PostAsync<FriendshipStatus>($"storytellers/{id}/add-to-friends", null).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> RejectFriendshipRequestAsync(string id, int notificationId)
        {
            var result = await this.PostAsync<FriendshipStatus>($"storytellers/{id}/reject-friendship", notificationId).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> AcceptFriendshipRequestAsync(string id, int notificationId)
        {
            var result = await this.PostAsync<FriendshipStatus>($"storytellers/{id}/add-to-friends", notificationId).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> SendRequestToJoinAsync(string email)
        {
            var result = await this.PostAsync<object>($"storytellers/request-to-join", email).ConfigureAwait(false);
            return result;
        }
    }
}