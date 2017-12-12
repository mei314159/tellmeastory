using System.Threading.Tasks;
using TellMe.Shared.Contracts.DTO;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface ITribeService : IService
    {
        Task<SharedTribeDTO> CreateAsync(string currentUserId, SharedTribeDTO dto);
        Task<SharedTribeDTO> UpdateAsync(string currentUserId, SharedTribeDTO dto);

        Task<TribeMemberStatus> RejectTribeInvitationAsync(string currentUserId, int tribeId);

        Task<TribeMemberStatus> AcceptTribeInvitationAsync(string currentUserId, int tribeId);

        Task LeaveTribeAsync(string currentUserId, int tribeId);

        Task<SharedTribeDTO> GetAsync(string userId, int tribeId);
        Task<bool> IsTribeCreatorAsync(string userId, int tribeId);
    }
}