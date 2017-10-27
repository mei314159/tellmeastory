using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface ITribeService : IService
    {
        Task<TribeDTO> CreateAsync(string currentUserId, TribeDTO dto);
        Task<TribeDTO> UpdateAsync(string currentUserId, TribeDTO dto);

        Task<TribeMemberStatus> RejectTribeInvitationAsync(string currentUserId, int tribeId);

        Task<TribeMemberStatus> AcceptTribeInvitationAsync(string currentUserId, int tribeId);
        
        Task<TribeDTO> GetAsync(string userId, int tribeId);
        Task<bool> IsTribeCreatorAsync(string userId, int tribeId);
    }
}