using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemoteNotificationsDataService : IRemoteDataService
    {
        Task<Result<List<NotificationDTO>>> GetNotificationsAsync(DateTime? olderThanUtc = null);
        Task<Result<int>> GetActiveNotificationsCountAsync();
        Task<Result> HandleNotificationAsync(int notificationId);
    }
}