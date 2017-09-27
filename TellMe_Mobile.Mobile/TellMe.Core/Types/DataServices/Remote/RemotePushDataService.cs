using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemotePushDataService : BaseDataService
    {
        public async Task<Result> RegisterAsync(string oldToken, string newToken)
        {
            var uri = new Uri(Constants.ApiHost + "/api/Push/RegisterToken");

            var data = new Dictionary<string, string>();
            data.Add("oldToken", oldToken);
            data.Add("token", newToken);
            data.Add("osType", App.Instance.OsType.ToString());
            data.Add("appVersion", App.Instance.AppVersion);

            var result = await this.SendDataAsync<object>("Push/RegisterToken", HttpMethod.Post, new FormUrlEncodedContent(data))
                                   .ConfigureAwait(false);
            return result;
        }
    }
}