using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemotePushDataService : IRemoteDataService
    {
        Task<Result> RegisterAsync(string oldToken, string newToken);
    }
}