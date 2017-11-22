using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.PushNotifications;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Event, int> _eventsRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        private readonly IRepository<EventAttendee, int> _eventAttendeesRepository;
        private readonly IRepository<StoryRequest, int> _storyRequestRepository;

        public EventService(IUnitOfWork unitOfWork, IRepository<Event, int> eventsRepository,
            IPushNotificationsService pushNotificationsService, IRepository<Notification, int> notificationRepository,
            IRepository<TribeMember, int> tribeMemberRepository,
            IRepository<EventAttendee, int> eventAttendeesRepository,
            IRepository<StoryRequest, int> storyRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _eventsRepository = eventsRepository;
            _pushNotificationsService = pushNotificationsService;
            _notificationRepository = notificationRepository;
            _tribeMemberRepository = tribeMemberRepository;
            _eventAttendeesRepository = eventAttendeesRepository;
            _storyRequestRepository = storyRequestRepository;
        }

        public async Task<ICollection<EventDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc)
        {
            var tribeMembers = _tribeMemberRepository.GetQueryable(true).Where(x => x.UserId == currentUserId);
            var attendees = _eventAttendeesRepository.GetQueryable(true);

            attendees = from attendee in attendees
                join tribeMember in tribeMembers
                    on attendee.TribeId equals tribeMember.TribeId into gj
                from tb in gj.DefaultIfEmpty()
                where
                    attendee.Status != EventAttendeeStatus.Rejected &&
                    (attendee.UserId == currentUserId ||
                     tb != null && (tb.Status == TribeMemberStatus.Joined || tb.Status == TribeMemberStatus.Creator))
                select
                    attendee;

            IQueryable<Event> events = _eventsRepository
                .GetQueryable(true)
                .Include(x => x.Host)
                .Include(x => x.Attendees).ThenInclude(x => x.User)
                .Include(x => x.Attendees).ThenInclude(x => x.Tribe);
            events = (from @event in events
                    join attendee in attendees
                        on @event.Id equals attendee.EventId into gj
                    from st in gj.DefaultIfEmpty()
                    where @event.CreateDateUtc < olderThanUtc && (@event.HostId == currentUserId || st != null)
                    orderby @event.CreateDateUtc descending
                    select @event)
                .Take(20);


            var list = await events.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<EventDTO>>(list);

            return result;
        }

        public async Task<EventDTO> CreateAsync(string currentUserId, EventDTO newEventDTO)
        {
            _unitOfWork.BeginTransaction();
            var now = DateTime.UtcNow;
            var entity = Mapper.Map<Event>(newEventDTO);
            entity.HostId = currentUserId;
            entity.CreateDateUtc = now;
            await _eventsRepository.SaveAsync(entity, true).ConfigureAwait(false);

            var entities = newEventDTO.Attendees.Select(x => new StoryRequest
            {
                CreateDateUtc = now,
                SenderId = currentUserId,
                UserId = x.TribeId == null ? x.UserId : null,
                TribeId = x.TribeId,
                Title = newEventDTO.StoryRequestTitle
            });
            
            _storyRequestRepository.AddRange(entities, true);

            var entityId = entity.Id;
            entity = await _eventsRepository
                .GetQueryable(true)
                .Include(x => x.Host)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == entityId)
                .ConfigureAwait(false);

            var eventDTO = Mapper.Map<EventDTO>(entity);
            var tribeIds = eventDTO.Attendees.Where(x => x.TribeId != null).Select(x => x.TribeId).ToList();
            var tribeMembers = await _tribeMemberRepository.GetQueryable(true).Where(x => tribeIds.Contains(x.TribeId))
                .Select(x => new
                {
                    x.TribeId,
                    TribeName = x.Tribe.Name,
                    x.UserId
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var notifications = eventDTO.Attendees.SelectMany(receiver =>
            {
                if (receiver.TribeId == null)
                {
                    return new[]
                    {
                        new Notification
                        {
                            Date = now,
                            Type = NotificationTypeEnum.Event,
                            RecipientId = receiver.UserId,
                            Extra = eventDTO,
                            Text = $"{eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                        }
                    };
                }

                return tribeMembers.Where(x => x.TribeId == receiver.TribeId).Select(x => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.Event,
                    RecipientId = x.UserId,
                    Extra = eventDTO,
                    Text =
                        $"[{x.TribeName}]: {eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                });
            }).ToArray();

            _notificationRepository.AddRange(notifications, true);
            _unitOfWork.SaveChanges();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
            return eventDTO;
        }

        public Task<EventDTO> EditAsync(string currentUserId, EventDTO eventDTO)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string currentUserId, int eventId)
        {
            _unitOfWork.BeginTransaction();

            var comment = await _eventsRepository
                .GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == eventId && x.HostId == currentUserId)
                .ConfigureAwait(false);

            if (comment == null)
            {
                throw new Exception("Comment was not found or you don't have permissions to delete it");
            }

            _eventsRepository.Remove(comment, true);
            _unitOfWork.SaveChanges();
        }
    }
}