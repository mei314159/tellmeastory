using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Types.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification, int> _notificationRepository;

        public NotificationService(IRepository<Notification, int> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IReadOnlyCollection<NotificationDTO>> GetNotificationsAsync(string currentUserId, DateTime olderThanUtc)
        {
            var query = await _notificationRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.Date < olderThanUtc && x.RecipientId == currentUserId)
            .OrderByDescending(x => x.Date)
            .Take(10)
            .ToListAsync()
            .ConfigureAwait(false);

            var result = Mapper.Map<List<NotificationDTO>>(query);
            return result;
        }

        public async Task<int> GetActiveNotificationsCountAsync(string currentUserId)
        {
            var result = await _notificationRepository
            .GetQueryable()
            .AsNoTracking()
            .CountAsync(x => x.RecipientId == currentUserId && !x.Handled)
            .ConfigureAwait(false);

            return result;
        }


        public async Task HandleNotificationAsync(string currentUserId, int notificationId)
        {
            var notification = await _notificationRepository
            .GetQueryable()
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.RecipientId == currentUserId)
            .ConfigureAwait(false);
            if (notification == null)
            {
                throw new Exception("Notification doesn't exist or your are not allowed to change it");
            }

            notification.Handled = true;
            await _notificationRepository
            .SaveAsync(notification, true)
            .ConfigureAwait(false);
        }
    }
}