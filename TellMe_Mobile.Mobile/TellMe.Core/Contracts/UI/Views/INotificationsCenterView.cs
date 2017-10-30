using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface INotificationsCenterView : IView
    {
        void DisplayNotifications(IReadOnlyCollection<NotificationDTO> notifications);
        void ReloadNotification(NotificationDTO notification);
    }
}