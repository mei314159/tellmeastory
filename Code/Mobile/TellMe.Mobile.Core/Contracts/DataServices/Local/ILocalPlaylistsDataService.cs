using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalPlaylistsDataService : ILocalDataService
    {
        Task<DataResult<PlaylistDTO>> GetAsync(int id);
        
        Task SaveAsync(PlaylistDTO entity);
        
        Task SaveAllAsync(IEnumerable<PlaylistDTO> entities);
        
        Task DeleteAsync(PlaylistDTO dto);
    }
}