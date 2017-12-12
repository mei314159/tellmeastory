﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemoteEventsDataService : IRemoteDataService
    {
        Task<Result<EventDTO>> GetEventAsync(int id);

        Task<Result<List<EventDTO>>> GetEventsAsync(DateTime? olderThanUtc = null);

        Task<Result<EventDTO>> SaveEventAsync(EventDTO eventDTO);

        Task<Result> DeleteEventAsync(int id);
    }
}