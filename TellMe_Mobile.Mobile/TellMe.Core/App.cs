using System;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Types.DataServices.Local;

namespace TellMe.Core
{
    public sealed class App
    {
		private AccountService _localAccountService;
        private IApplicationDataStorage _dataStorage;

        private static readonly Lazy<App> lazy = new Lazy<App>(() => new App());
        public static App Instance => lazy.Value;

        private App()
        {
        }

        public void Initialize(AccountService localAccountService, IApplicationDataStorage dataStorage, IRouter router)
        {
            _localAccountService = localAccountService;
            _dataStorage = dataStorage;
            Router = router;
        }



        public IRouter Router { get; set; }

        public OsType OsType => _dataStorage.OsType;

        public string AppVersion => _dataStorage.AppVersion;

        public AuthenticationInfoDTO AuthInfo
        {
            get
            {
                return _localAccountService.GetAuthInfo();
            }
            set
            {
                _localAccountService.SaveAuthInfo(value);
            }
        }

        public event Action<Exception> OnException;
        public event Action<Exception> OnNetworkException;


        public void LogException(Exception ex)
        {
            OnException?.Invoke(ex);
        }

        public void LogNetworkException(Exception ex)
        {
            OnNetworkException?.Invoke(ex);
        }
    }
}
