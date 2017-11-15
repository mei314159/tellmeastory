using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemotePushDataService : IRemoteDataService
    {
        Task<Result> RegisterAsync(string oldToken, string newToken);
    }
}