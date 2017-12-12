using System.Threading.Tasks;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface ITribeService : IService
    {
        Task<TribeDTO> CreateAsync(string currentUserId, TribeDTO dto);
        Task<TribeDTO> UpdateAsync(string currentUserId, TribeDTO dto);

        Task<TribeMemberStatus> RejectTribeInvitationAsync(string currentUserId, int tribeId);

        Task<TribeMemberStatus> AcceptTribeInvitationAsync(string currentUserId, int tribeId);

        Task LeaveTribeAsync(string currentUserId, int tribeId);

        Task<TribeDTO> GetAsync(string userId, int tribeId);
        Task<bool> IsTribeCreatorAsync(string userId, int tribeId);
    }
}