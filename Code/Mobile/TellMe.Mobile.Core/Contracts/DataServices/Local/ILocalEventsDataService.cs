using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalEventsDataService : ILocalDataService
    {
        Task<DataResult<EventDTO>> GetAsync(int id);
        
        Task SaveAsync(EventDTO entity);
        
        Task SaveAllAsync(IEnumerable<EventDTO> entities);
        
        Task DeleteAsync(EventDTO dto);
    }
} 