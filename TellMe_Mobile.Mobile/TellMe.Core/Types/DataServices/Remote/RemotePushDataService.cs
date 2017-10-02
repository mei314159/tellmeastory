using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemotePushDataService : BaseDataService
    {
        public async Task<Result> RegisterAsync(string oldToken, string newToken)
        {
            var result = await this.PostAsync<object>("push/register-token", new PushTokenDTO
            {
                OsType = App.Instance.OsType,
                AppVersion = App.Instance.AppVersion,
                Token = newToken,
                OldToken = oldToken
            }).ConfigureAwait(false);
            return result;
        }
    }
}