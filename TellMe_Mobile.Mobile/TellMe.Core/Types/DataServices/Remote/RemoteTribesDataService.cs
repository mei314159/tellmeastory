using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteTribesDataService : BaseDataService
    {
        public async Task<Result<TribeDTO>> CreateAsync(TribeDTO dto)
        {
            var result = await this.PostAsync<TribeDTO>("tribes", dto).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<TribeMemberStatus>> RejectTribeInvitationAsync(int tribeId, int? notificationId = null)
        {
            var result = await this.PostAsync<TribeMemberStatus>($"tribes/{tribeId}/reject", notificationId).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<TribeMemberStatus>> AcceptTribeInvitationAsync(int tribeId, int? notificationId = null)
        {
            var result = await this.PostAsync<TribeMemberStatus>($"tribes/{tribeId}/join", notificationId).ConfigureAwait(false);
            return result;
        }
    }
}