using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts
{
    public interface INotificationHandler
    {
        Task<bool?> ProcessNotificationAsync(NotificationDTO notification, IView view);
    }
}