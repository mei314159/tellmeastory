using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
{
    public class RemoteEventsDataService : IRemoteEventsDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteEventsDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<EventDTO>> GetEventAsync(int id)
        {
            var result = await this._apiProvider.GetAsync<EventDTO>($"events/{id}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<EventDTO>>> GetEventsAsync(DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider.GetAsync<List<EventDTO>>($"events/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<EventDTO>> SaveEventAsync(EventDTO eventDTO)
        {
            var httpMethod = eventDTO.Id == default(int) ? HttpMethod.Post : HttpMethod.Put;
            var result = await this._apiProvider.SendDataAsync<EventDTO>("events", eventDTO, httpMethod).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> DeleteEventAsync(int id)
        {
            var result = await this._apiProvider.DeleteAsync<object>($"events/{id}", null).ConfigureAwait(false);
            return result;
        }
    }
}