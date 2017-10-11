using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;

namespace TellMe.DAL.Types.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<ApplicationUser, string> _userRepository;
        private readonly IRepository<RefreshToken, int> _refreshTokenRepository;

        public UserService(IRepository<ApplicationUser, string> serviceRepository, IRepository<RefreshToken, int> refreshTokenRepository)
        {
            _userRepository = serviceRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<ApplicationUser> GetAsync(string id)
        {
            var result = await _userRepository.GetQueryable().SingleAsync(a => a.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<bool> AddTokenAsync(RefreshToken token)
        {
            await _refreshTokenRepository.SaveAsync(token, true).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> ExpireTokenAsync(RefreshToken token)
        {
            await _refreshTokenRepository.SaveAsync(token).ConfigureAwait(false);
            return true;
        }

        public Task<RefreshToken> GetTokenAsync(string token, string clientId)
        {
            return _refreshTokenRepository.GetQueryable().FirstOrDefaultAsync(x => x.ClientId == clientId && x.Token == token);
        }
    }
}