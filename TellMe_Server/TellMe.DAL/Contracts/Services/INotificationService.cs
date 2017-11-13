using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface INotificationService : IService
    {
        Task<IReadOnlyCollection<NotificationDTO>> GetNotificationsAsync(string currentUserId, DateTime olderThanUtc);
        Task<int> GetActiveNotificationsCountAsync(string currentUserId);
        Task HandleNotificationAsync(string currentUserId, int notificationId);
    }
}