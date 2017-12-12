using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalNotificationsDataService : ILocalDataService
    {
        Task DeleteAllAsync();
        Task SaveAllAsync(ICollection<NotificationDTO> entities);
        Task<DataResult<NotificationDTO[]>> GetAllAsync();
        Task SaveAsync(NotificationDTO notification);
    }
}