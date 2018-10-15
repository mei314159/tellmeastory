using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Shared.Contracts.DTO;
using TellMe.Web.DAL.DTO;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface IPlaylistService : IService
    {
        Task<PlaylistDTO> GetAsync(string currentUserId, int playlistId);
        
        Task<ICollection<PlaylistDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc);
        
        Task<PlaylistDTO> SaveAsync(string currentUserId, PlaylistDTO dto);
        
        Task ShareAsync(string currentUserId, int playlistId, SharePlaylistDTO dto);
               
        Task DeleteAsync(string currentUserId, int playlistId);
    }
}