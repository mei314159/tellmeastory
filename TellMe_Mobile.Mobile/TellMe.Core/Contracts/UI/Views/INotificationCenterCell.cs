using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface INotificationCenterCell
    {
        NotificationDTO Notification { get; set; }
    }
}