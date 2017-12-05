using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
{
    public class RemotePlaylistsDataService : IRemotePlaylistsDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemotePlaylistsDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<PlaylistDTO>> GetAsync(int id)
        {
            var result = await this._apiProvider.GetAsync<PlaylistDTO>($"playlists/{id}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<PlaylistDTO>>> GetPlaylistsAsync(DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider.GetAsync<List<PlaylistDTO>>($"playlists/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<PlaylistDTO>> SaveAsync(PlaylistDTO dto)
        {
            var httpMethod = dto.Id == default(int) ? HttpMethod.Post : HttpMethod.Put;
            var result = await this._apiProvider.SendDataAsync<PlaylistDTO>("playlists", dto, httpMethod).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var result = await this._apiProvider.DeleteAsync<object>($"playlists/{id}", null).ConfigureAwait(false);
            return result;
        }
    }
}