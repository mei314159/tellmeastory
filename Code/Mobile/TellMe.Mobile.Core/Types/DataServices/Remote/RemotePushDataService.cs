using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
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