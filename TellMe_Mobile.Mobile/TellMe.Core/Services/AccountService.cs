using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using TellMe.Core.Contracts;
using TellMe.Core.DTO;
using TellMe.Core.Dto;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Validation;

namespace TellMe.Core.Services
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

        public async Task<ServiceResult<AuthenticationInfoDTO>> SignInAsync(ISigninView viewModel)
        {
            var validationResult = await new SignInValidator().ValidateAsync(viewModel).ConfigureAwait(false);
            Result<AuthenticationInfoDTO> result = null;
            if (validationResult.IsValid)
            {
				var data = new Dictionary<string, string>();
				data.Add("grant_type", "password");
				data.Add("username", viewModel.EmailField.Text);
				data.Add("password", viewModel.PasswordField.Text);
				data.Add("client_id", "ios_app");
                result = await this.SendDataAsync<AuthenticationInfoDTO>("token/auth", HttpMethod.Post, new FormUrlEncodedContent(data), true)
                                   .ConfigureAwait(false);
            }

            return new ServiceResult<AuthenticationInfoDTO>(validationResult, result);
        }

        //public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        //{
        //    return await PostAsync<object>("Account/ResetPassword", dto, true);
        //}

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            return await PostAsync<object>("Account/ForgotPassword", new { email }, true);
        }

        public async Task<ServiceResult> SignUpAsync(ISignupView viewModel)
        {
            var validationResult = await new SignUpValidator().ValidateAsync(viewModel).ConfigureAwait(false);
            Result<object> result = null;

            if (validationResult.IsValid)
            {
                result = await this.PostAsync<object>("account/signup", new SignUpDTO
                {
                    Email = viewModel.EmailField.Text,
                    Password = viewModel.PasswordField.Text,
                    ConfirmPassword = viewModel.ConfirmPasswordField.Text
                }, true).ConfigureAwait(false);
            }

            ServiceResult serviceResult = new ServiceResult(validationResult, result);
            return serviceResult;
        }

        public void SignOut()
        {
            this.ApplicationDataStorage.AuthInfo = null;
        }
    }
}
