using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalTribesDataService : ILocalDataService
    {
        Task DeleteAllAsync();
        Task DeleteAsync(TribeDTO dto);
        Task SaveAllAsync(IEnumerable<TribeDTO> items);
        Task<DataResult<TribeDTO>> GetAsync(int tribeId);
        Task<DataResult<ICollection<TribeDTO>>> GetAllAsync();
        Task SaveAsync(TribeDTO item);
    }
}