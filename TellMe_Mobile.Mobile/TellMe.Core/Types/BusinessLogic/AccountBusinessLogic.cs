using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;

namespace TellMe.Core.Types.BusinessLogic
{
    public class AccountBusinessLogic : IAccountBusinessLogic
    {
        private const string PushTokenKey = "PushToken";
        private const string OldPushTokenKey = "OldPushToken";
        private const string PushTokenSentToBackendKey = "PushTokenSentToBackend";
        private const string PushIsEnabledKey = "PushIsEnabled";

        private readonly IApplicationDataStorage _appDataStorage;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRemotePushDataService _remotePushDataService;

        public AccountBusinessLogic(IApplicationDataStorage appDataStorage, ILocalAccountService localAccountService, IRemotePushDataService remotePushDataService)
        {
            this._appDataStorage = appDataStorage;
            this._localAccountService = localAccountService;
            this._remotePushDataService = remotePushDataService;
        }

        public bool IsAuthenticated => _localAccountService.GetAuthInfo() != null;

        public bool PushIsEnabled
        {
            get => _appDataStorage.GetBool(PushIsEnabledKey);
            set => _appDataStorage.SetBool(PushIsEnabledKey, value);
        }

        public async Task RegisteredForRemoteNotificationsAsync(string pushToken)
        {
            string oldDeviceToken = _appDataStorage.Get<string>(PushTokenKey);

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(pushToken))
            {
                _appDataStorage.SetBool(PushTokenSentToBackendKey, false);
                _appDataStorage.Set(OldPushTokenKey, oldDeviceToken);
                _appDataStorage.Set(PushTokenKey, pushToken);
            }

			await SyncPushTokenAsync().ConfigureAwait(false);
        }

        public async Task SyncPushTokenAsync()
        {
            string pushToken = _appDataStorage.Get<string>(PushTokenKey);
            if (!string.IsNullOrWhiteSpace(pushToken) && this.IsAuthenticated && !_appDataStorage.GetBool(PushTokenSentToBackendKey))
            {
                string oldDeviceToken = _appDataStorage.Get<string>(OldPushTokenKey);
                var result = await _remotePushDataService.RegisterAsync(oldDeviceToken, pushToken).ConfigureAwait(false);
                _appDataStorage.SetBool(PushTokenSentToBackendKey, result.IsSuccess);
            }
        }
    }
}
