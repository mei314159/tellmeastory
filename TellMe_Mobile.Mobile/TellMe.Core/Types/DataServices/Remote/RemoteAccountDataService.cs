using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteAccountDataService : BaseDataService
    {
        public async Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> SignInAsync(string email, string password)
        {
            var data = new Dictionary<string, string>();
            data.Add("grant_type", "password");
            data.Add("username", email);
            data.Add("password", password);
            data.Add("client_id", "ios_app");
            var result = await this.SendDataAsync<AuthenticationInfoDTO, AuthenticationErrorDto>("token/auth", HttpMethod.Post, new FormUrlEncodedContent(data), true)
                                   .ConfigureAwait(false);
            return result;
        }

        //public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        //{
        //    return await PostAsync<object>("Account/ResetPassword", dto, true);
        //}

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            return await PostAsync<object>("Account/ForgotPassword", new { email }, true);
        }

        public async Task<Result> SignUpAsync(SignUpDTO dto)
        {
            var result = await this.PostAsync<object>("account/signup", dto, true).ConfigureAwait(false);
            return result;
        }
    }
}
