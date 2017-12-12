using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface INotificationCenterBusinessLogic : IBusinessLogic
    {
        INotificationsCenterView View { get; set; }
        Task LoadNotificationsAsync(bool forceRefresh = false);
        void HandleNotification(NotificationDTO notification);
    }
}