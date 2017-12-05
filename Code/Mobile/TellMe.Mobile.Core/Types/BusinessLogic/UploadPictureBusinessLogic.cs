using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class UploadPictureBusinessLogic : IUploadPictureBusinessLogic
    {
        private readonly IRemoteAccountDataService _remoteAccountDataService;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRouter _router;

        public UploadPictureBusinessLogic(IRemoteAccountDataService remoteAccountDataService,
            ILocalAccountService localAccountService, IRouter router)
        {
            this._remoteAccountDataService = remoteAccountDataService;
            this._localAccountService = localAccountService;
            this._router = router;
        }

        public IUploadPictureView View { get; set; }

        public void Init()
        {
            var account = _localAccountService.GetAuthInfo().Account;
            this.View.ProfilePicture.SetPictureUrl(account.PictureUrl, null);
        }

        public void SkipButtonTouched()
        {
            _router.NavigateMain();
        }

        public void SelectPictureTouched()
        {
            this.View.ShowPictureSourceDialog();
        }

        public async Task PictureSelectedAsync()
        {
            await SetProfilePictureAsync().ConfigureAwait(false);
        }

        private async Task SetProfilePictureAsync()
        {
            var stream = View.ProfilePicture.GetPictureStream();
            var result = await _remoteAccountDataService.SetProfilePictureAsync(stream).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                stream.Dispose();
                var authInfo = _localAccountService.GetAuthInfo();
                authInfo.Account.PictureUrl = result.Data.PictureUrl;
                _localAccountService.SaveAuthInfo(authInfo);
                _router.NavigateMain();
                return;
            }

            result.ShowResultError(this.View);
        }
    }
}