using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteEventsService : IRemoteDataService
    {
        Task<Result<List<EventDTO>>> GetEventsAsync(DateTime? olderThanUtc = null);
        
        Task<Result<EventDTO>> CreateEventAsync(EventDTO eventDTO);
        
        Task<Result<EventDTO>> EditEventAsync(EventDTO eventDTO);
    }
}