using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
{
    public class RemoteAccountDataService : IRemoteAccountDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteAccountDataService(IApiProvider apiProvider)
        {
            this._apiProvider = apiProvider;
        }

        public async Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> SignInAsync(string email,
            string password)
        {
            var data = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", email},
                {"password", password},
                {"client_id", "ios_app"}
            };
            var result = await this._apiProvider
                .SendDataAsync<AuthenticationInfoDTO, AuthenticationErrorDto>("token/auth", HttpMethod.Post,
                    new FormUrlEncodedContent(data), true)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result> SignUpAsync(SignUpDTO dto)
        {
            var result = await this._apiProvider.PostAsync<object>("account/signup", dto, true).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<ProfilePictureDTO>> SetProfilePictureAsync(Stream profilePictureStream)
        {
            profilePictureStream.Position = 0;

            var data = new MultipartFormDataContent
            {
                {new StreamContent(profilePictureStream), "File", Guid.NewGuid() + ".jpg"}
            };

            var result = await this._apiProvider
                .SendDataAsync<ProfilePictureDTO>("account/picture", HttpMethod.Post, data).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<UserDTO>> SaveAsync(UserDTO dto, Stream profilePictureStream = null)
        {
            profilePictureStream.Position = 0;
            var serializedData = JsonConvert.SerializeObject(dto);
            var data = new MultipartFormDataContent
            {
                {new StringContent(serializedData, System.Text.Encoding.UTF8, "application/json"), "User"}
            };
            if (profilePictureStream != null)
            {
                data.Add(new StreamContent(profilePictureStream), "File", Guid.NewGuid() + ".jpg");
            }

            var result = await this._apiProvider.SendDataAsync<UserDTO>("account", HttpMethod.Post, data).ConfigureAwait(false);
            return result;
        }
    }
}