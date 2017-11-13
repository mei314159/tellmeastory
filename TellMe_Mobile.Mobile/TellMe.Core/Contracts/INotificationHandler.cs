using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts
{
    public interface INotificationHandler
    {
        Task<bool?> ProcessNotification(NotificationDTO notification);
    }
}