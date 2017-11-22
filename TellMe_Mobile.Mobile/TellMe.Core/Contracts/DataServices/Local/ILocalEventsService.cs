using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Local
{
    public interface ILocalEventsService : ILocalDataService
    {
        Task SaveAllAsync(IEnumerable<EventDTO> entities);
    }
}