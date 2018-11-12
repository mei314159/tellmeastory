using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemotePlaylistsDataService : IRemoteDataService
    {
        Task<Result<PlaylistDTO>> GetAsync(int id);

        Task<Result<List<PlaylistDTO>>> GetPlaylistsAsync(DateTime? olderThanUtc = null);
        
        Task<Result<List<StoryDTO>>> GetStoriesAsync(int playlistId);

        Task<Result<PlaylistDTO>> SaveAsync(PlaylistDTO dto);
        
        Task<Result> ShareAsync(SharePlaylistDTO dto);

        Task<Result> DeleteAsync(int id);
    }
}