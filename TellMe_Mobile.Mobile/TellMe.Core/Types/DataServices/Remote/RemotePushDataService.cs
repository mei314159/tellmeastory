using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemotePushDataService : IRemotePushDataService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IApplicationDataStorage _dataStorage;

        public RemotePushDataService(IApiProvider apiProvider, IApplicationDataStorage dataStorage)
        {
            _apiProvider = apiProvider;
            _dataStorage = dataStorage;
        }

        public async Task<Result> RegisterAsync(string oldToken, string newToken)
        {
            var result = await this._apiProvider.PostAsync<object>("push/register-token", new PushTokenDTO
            {
                OsType = _dataStorage.OsType,
                AppVersion = _dataStorage.AppVersion,
                Token = newToken,
                OldToken = oldToken
            }).ConfigureAwait(false);
            return result;
        }
    }
}