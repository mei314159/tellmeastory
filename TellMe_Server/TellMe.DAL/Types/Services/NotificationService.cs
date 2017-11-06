using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TellMe.DAL.Contracts.DTO;
using AutoMapper;
using System;

namespace TellMe.DAL.Types.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification, int> _notificationRepository;

        public NotificationService(IRepository<Notification, int> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IReadOnlyCollection<NotificationDTO>> GetNotificationsAsync(string currentUserId, int skip)
        {
            var query = await _notificationRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.RecipientId == currentUserId)
            .OrderByDescending(x => x.Date)
            .Skip(skip).Take(10)
            .ToListAsync()
            .ConfigureAwait(false);

            var result = Mapper.Map<List<NotificationDTO>>(query);
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