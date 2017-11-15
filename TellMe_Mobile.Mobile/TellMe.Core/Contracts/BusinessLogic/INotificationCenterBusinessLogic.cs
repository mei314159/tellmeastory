using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface INotificationCenterBusinessLogic : IBusinessLogic
    {
        INotificationsCenterView View { get; set; }
        Task LoadNotificationsAsync(bool forceRefresh = false);
        void HandleNotification(NotificationDTO notification);
    }
}