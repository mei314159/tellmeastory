using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalAccountService : ILocalDataService
    {
        void SaveAuthInfo(AuthenticationInfoDTO authInfo);
        AuthenticationInfoDTO GetAuthInfo();
    }
}