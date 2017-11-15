using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteAccountDataService : IRemoteAccountDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteAccountDataService(IApiProvider apiProvider)
        {
            this._apiProvider = apiProvider;
        }

        public async Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> SignInAsync(string email, string password)
        {
            var data = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", email},
                {"password", password},
                {"client_id", "ios_app"}
            };
            var result = await this._apiProvider.SendDataAsync<AuthenticationInfoDTO, AuthenticationErrorDto>("token/auth", HttpMethod.Post, new FormUrlEncodedContent(data), true)
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

            var result = await this._apiProvider.SendDataAsync<ProfilePictureDTO>("account/picture", HttpMethod.Post, data).ConfigureAwait(false);
            return result;
        }
    }
}
