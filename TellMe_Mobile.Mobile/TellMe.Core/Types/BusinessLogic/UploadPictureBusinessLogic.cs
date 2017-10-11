using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class UploadPictureBusinessLogic
    {
        private readonly RemoteAccountDataService _remoteAccountDataService;
        private readonly AccountService _accountService;
        private readonly IUploadPictureView _view;
        private readonly IRouter _router;

        public UploadPictureBusinessLogic(RemoteAccountDataService _remoteAccountDataService, AccountService _accountService, IUploadPictureView _view, IRouter _router)
        {
            this._remoteAccountDataService = _remoteAccountDataService;
            this._accountService = _accountService;
            this._view = _view;
            this._router = _router;
        }

        public void Init()
        {
            var account = _accountService.GetAuthInfo().Account;
            this._view.ProfilePicture.SetPictureUrl(account.PictureUrl);
        }

        public void SkipButtonTouched()
        {
            _router.NavigateMain();
        }

        public void SelectPictureTouched()
        {
            this._view.ShowPictureSourceDialog();
        }

        public async Task PictureSelectedAsync()
        {
            await SetProfilePictureAsync().ConfigureAwait(false);
        }

        private async Task SetProfilePictureAsync()
        {
            var stream = _view.ProfilePicture.GetPictureStream();
            var result = await _remoteAccountDataService.SetProfilePictureAsync(stream).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                stream.Dispose();
                var authInfo = _accountService.GetAuthInfo();
                authInfo.Account.PictureUrl = result.Data.PictureUrl;
                _accountService.SaveAuthInfo(authInfo);
                _router.NavigateMain();
                return;
            }

            result.ShowResultError(this._view);
        }
    }
}
