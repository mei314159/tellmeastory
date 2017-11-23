using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
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
            var result = await this._apiProvider.PostAsync<EventDTO>("events", eventDTO).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> DeleteEventAsync(int id)
        {
            var result = await this._apiProvider.DeleteAsync<object>($"events/{id}", null).ConfigureAwait(false);
            return result;
        }
    }
}