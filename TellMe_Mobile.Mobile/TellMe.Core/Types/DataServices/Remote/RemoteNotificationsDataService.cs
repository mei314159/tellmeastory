using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteNotificationsDataService : BaseDataService
    {
        public async Task<Result<List<NotificationDTO>>> GetNotificationsAsync(DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this.GetAsync<List<NotificationDTO>>($"notifications/older-than/{olderThan.Ticks}").ConfigureAwait(false);
            return result;
        }

        public async Task<Result<int>> GetActiveNotificationsCountAsync()
        {
            var result = await this.GetAsync<int>($"notifications/active/count").ConfigureAwait(false);
            return result;
        }

        public async Task<Result> HandleNotificationAsync(int notificationId)
        {
            var result = await this.PostAsync<List<NotificationDTO>>($"notifications/{notificationId}/handled", null).ConfigureAwait(false);
            return result;
        }
    }
}
