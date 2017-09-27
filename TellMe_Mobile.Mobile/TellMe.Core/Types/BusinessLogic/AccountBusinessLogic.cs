using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.Core.Types.BusinessLogic
{
    public class AccountBusinessLogic
    {
        private const string PushTokenKey = "PushToken";
        private const string OldPushTokenKey = "OldPushToken";
        private const string PushTokenSentToBackendKey = "PushTokenSentToBackend";
        private const string PushIsEnabledKey = "PushIsEnabled";


        private readonly IApplicationDataStorage _appDataStorage;
        private readonly AccountService _accountService;
        private readonly RemotePushDataService _remotePushDataService;

        public AccountBusinessLogic(IApplicationDataStorage _appDataStorage, AccountService _accountService, RemotePushDataService _remotePushDataService)
        {
            this._appDataStorage = _appDataStorage;
            this._accountService = _accountService;
            this._remotePushDataService = _remotePushDataService;
        }

        public bool IsAuthenticated => _accountService.GetAuthInfo() != null;

        public bool PushIsEnabled
        {
            get
            {
                return _appDataStorage.GetBool(PushIsEnabledKey);
            }
            set
            {
                _appDataStorage.SetBool(PushIsEnabledKey, value);
            }
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

                await SyncPushTokenAsync().ConfigureAwait(false);
            }
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
