using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemoteTribesDataService : IRemoteDataService
    {
        Task<Result<TribeDTO>> CreateAsync(TribeDTO dto);
        Task<Result<TribeMemberStatus>> RejectTribeInvitationAsync(int tribeId, int? notificationId = null);
        Task<Result<TribeMemberStatus>> AcceptTribeInvitationAsync(int tribeId, int? notificationId = null);
        Task<Result<TribeDTO>> GetTribeAsync(int tribeId);
        Task<Result<TribeDTO>> UpdateAsync(TribeDTO dto);
        Task<Result> LeaveAsync(int tribeId);
    }
}