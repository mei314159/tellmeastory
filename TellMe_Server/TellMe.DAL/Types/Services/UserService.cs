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

        public bool AddToken(RefreshToken token)
        {
            _refreshTokenRepository.Save(token, true);
            return true;
        }

        public bool ExpireToken(RefreshToken token)
        {
            _refreshTokenRepository.Save(token);
            return true;
        }

        public RefreshToken GetToken(string token, string clientId)
        {
            return _refreshTokenRepository.GetQueryable().FirstOrDefault(x => x.ClientId == clientId && x.Token == token);
        }
    }
}