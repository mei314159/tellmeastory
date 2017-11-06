using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteNotificationsDataService : BaseDataService
    {
        public async Task<Result<List<NotificationDTO>>> GetNotificationsAsync(int skip)
        {
            var result = await this.GetAsync<List<NotificationDTO>>($"notifications/skip/{skip}").ConfigureAwait(false);
            return result;
        }

        public async Task<Result> HandleNotificationAsync(int notificationId)
        {
            var result = await this.GetAsync<List<NotificationDTO>>($"notifications/{notificationId}/handled").ConfigureAwait(false);
            return result;
        }
    }
}
