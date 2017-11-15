using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Local
{
    public interface ILocalNotificationsDataService : ILocalDataService
    {
        Task DeleteAllAsync();
        Task SaveAllAsync(ICollection<NotificationDTO> entities);
        Task<DataResult<NotificationDTO[]>> GetAllAsync();
        Task SaveAsync(NotificationDTO notification);
    }
}