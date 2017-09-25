using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class AccountService : BaseDataService
    {
        public AccountService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public AuthenticationInfoDTO AuthInfo
        {
            get { return this.ApplicationDataStorage.AuthInfo; }
        }

        public event Action<AuthenticationInfoDTO> Authenticated;

        public bool IsAuthenticated()
        {
            return this.ApplicationDataStorage.AuthInfo != null && (DateTime.Now - this.ApplicationDataStorage.AuthInfo.AuthDate).TotalSeconds < this.ApplicationDataStorage.AuthInfo.ExpiresIn;
        }

        public async Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> SignInPhoneAsync(string phoneNumber, string confirmationCode)
        {
            var data = new Dictionary<string, string>();
            data.Add("grant_type", "phone_number");
            data.Add("phone_number", phoneNumber);
            data.Add("confirmation_code", confirmationCode);
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

        public async Task<Result> SignUpPhoneAsync(SignUpPhoneDTO dto)
        {
            var result = await this.PostAsync<object>("account/signup-phone", dto, true).ConfigureAwait(false);
            return result;
        }

        public void SignOut()
        {
            this.ApplicationDataStorage.AuthInfo = null;
        }
    }
}
