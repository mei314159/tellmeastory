using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteNotificationsDataService : IRemoteDataService
    {
        Task<Result<List<NotificationDTO>>> GetNotificationsAsync(DateTime? olderThanUtc = null);
        Task<Result<int>> GetActiveNotificationsCountAsync();
        Task<Result> HandleNotificationAsync(int notificationId);
    }
}