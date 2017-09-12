using System.Threading.Tasks;
using TellMe.DAL.Contracts.Domain;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IUserService : IService
    {
        Task<ApplicationUser> GetAsync(string id);

        bool AddToken(RefreshToken token);

        bool ExpireToken(RefreshToken token);

        RefreshToken GetToken(string refresh_token, string client_id);
    }
}