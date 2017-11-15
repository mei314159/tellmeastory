using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Local
{
    public interface ILocalAccountService : ILocalDataService
    {
        void SaveAuthInfo(AuthenticationInfoDTO authInfo);
        AuthenticationInfoDTO GetAuthInfo();
    }
}