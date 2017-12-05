﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemotePlaylistsDataService : IRemoteDataService
    {
        Task<Result<PlaylistDTO>> GetAsync(int id);

        Task<Result<List<PlaylistDTO>>> GetPlaylistsAsync(DateTime? olderThanUtc = null);

        Task<Result<PlaylistDTO>> SaveAsync(PlaylistDTO dto);

        Task<Result> DeleteAsync(int id);
    }
}