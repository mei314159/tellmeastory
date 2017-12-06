using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Contracts.PushNotifications
{
    public interface IPushNotificationsService
    {
        Task RegisterPushTokenAsync(string token, string oldToken, OsType osType, string userId, string appVersion);
        
		Task SendPushNotificationAsync(Notification notification);
        
        Task SendPushNotificationsAsync(ICollection<Notification> notifications);
    }
}