using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class AccountBusinessLogic : IAccountBusinessLogic
    {
        private const string PushTokenKey = "PushToken";
        private const string OldPushTokenKey = "OldPushToken";
        private const string PushTokenSentToBackendKey = "PushTokenSentToBackend";
        private const string PushIsEnabledKey = "PushIsEnabled";

        private readonly IApplicationDataStorage _appDataStorage;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRemoteAccountDataService _remoteAccountDataService;
        private readonly IRemotePushDataService _remotePushDataService;
        private readonly IRouter _router;
        private readonly UserValidator _validator;

        public AccountBusinessLogic(IApplicationDataStorage appDataStorage, ILocalAccountService localAccountService,
            IRemotePushDataService remotePushDataService, IRouter router, IRemoteAccountDataService remoteAccountDataService, UserValidator validator)
        {
            this._appDataStorage = appDataStorage;
            this._localAccountService = localAccountService;
            this._remotePushDataService = remotePushDataService;
            _router = router;
            _remoteAccountDataService = remoteAccountDataService;
            _validator = validator;
        }

        public IAccountView View { get; set; }

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
            if (!string.IsNullOrWhiteSpace(pushToken) && this.IsAuthenticated &&
                !_appDataStorage.GetBool(PushTokenSentToBackendKey))
            {
                string oldDeviceToken = _appDataStorage.Get<string>(OldPushTokenKey);
                var result = await _remotePushDataService.RegisterAsync(oldDeviceToken, pushToken)
                    .ConfigureAwait(false);
                _appDataStorage.SetBool(PushTokenSentToBackendKey, result.IsSuccess);
            }
        }

        public void SignOut()
        {
            _localAccountService.SaveAuthInfo(null);
            _router.SwapToAuth();
        }

        public void NavigateChangePicture()
        {
            _router.NavigateSetProfilePicture(this.View);
        }

        public void InitView()
        {
            this.View.User = _localAccountService.GetAuthInfo().Account;
            this.View.Display();
        }

        public async Task<bool> SaveAsync()
        {
            var validationResult = await this._validator.ValidateAsync(this.View.User).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                Stream stream = null;
                if (View.PictureChanged)
                {
                    stream = View.ProfilePicture.GetPictureStream();
                }

                var result = await _remoteAccountDataService.SaveAsync(this.View.User, stream)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    stream.Dispose();
                    View.User = result.Data;
                    var authInfo = this._localAccountService.GetAuthInfo();
                    authInfo.Account = result.Data;
                    _localAccountService.SaveAuthInfo(authInfo);
                    this.View.ShowSuccessMessage("Profile successfully saved", () => View.Display());
                    return true;
                }
                else
                {
                    result.ShowResultError(this.View);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this.View);
            }

            return false;
        }
    }
}