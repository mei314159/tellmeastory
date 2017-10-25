using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteStorytellersDataService : BaseDataService
    {
        public async Task<Result<List<ContactDTO>>> SearchAsync(string fragment, int skip, ContactsMode mode)
        {
            var url = string.IsNullOrWhiteSpace(fragment) 
                            ? $"storytellers/search/{mode}/skip/{skip}" 
                            : $"storytellers/search/{mode}/skip/{skip}/{fragment}";
            var result = await this.GetAsync<List<ContactDTO>>(url).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> SendFriendshipRequestAsync(string id)
        {
            var result = await this.PostAsync<FriendshipStatus>($"storytellers/{id}/add-to-friends", null).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> RejectFriendshipRequestAsync(string id, int notificationId)
        {
            var result = await this.PostAsync<FriendshipStatus>($"storytellers/{id}/reject-friendship", notificationId).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<FriendshipStatus>> AcceptFriendshipRequestAsync(string id, int notificationId)
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