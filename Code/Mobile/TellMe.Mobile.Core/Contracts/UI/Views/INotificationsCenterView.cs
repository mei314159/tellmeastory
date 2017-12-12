using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface INotificationsCenterView : IView
    {
        void DisplayNotifications(IReadOnlyCollection<NotificationDTO> notifications);
        void ReloadNotification(NotificationDTO notification);
    }
}