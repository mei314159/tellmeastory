using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts
{
    public interface INotificationHandler
    {
        Task<bool?> ProcessNotificationAsync(NotificationDTO notification, IView view);
    }
}