using System.Threading.Tasks;
using TellMe.DAL.Contracts.Domain;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IUserService : IService
    {
        Task<ApplicationUser> GetAsync(string id);

        Task<bool> AddTokenAsync(RefreshToken token);

        Task<bool> ExpireTokenAsync(RefreshToken token);

        Task<RefreshToken> GetTokenAsync(string token, string clientId);
    }
}