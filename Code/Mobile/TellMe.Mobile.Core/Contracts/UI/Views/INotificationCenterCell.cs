using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface INotificationCenterCell
    {
        NotificationDTO Notification { get; set; }
    }
}