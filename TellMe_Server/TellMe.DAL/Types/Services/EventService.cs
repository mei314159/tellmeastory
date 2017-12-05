using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.OData.Atom;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.PushNotifications;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Extensions;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Event, int> _eventsRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<TribeMember, int> _tribeMemberRepository;
        private readonly IRepository<EventAttendee, int> _eventAttendeeRepository;
        private readonly IRepository<StoryRequest, int> _storyRequestRepository;
        private readonly IRepository<Tribe, int> _tribeRepository;

        public EventService(IUnitOfWork unitOfWork, IRepository<Event, int> eventsRepository,
            IPushNotificationsService pushNotificationsService,
            IRepository<TribeMember, int> tribeMemberRepository,
            IRepository<EventAttendee, int> eventAttendeeRepository,
            IRepository<StoryRequest, int> storyRequestRepository, IRepository<Tribe, int> tribeRepository)
        {
            _unitOfWork = unitOfWork;
            _eventsRepository = eventsRepository;
            _pushNotificationsService = pushNotificationsService;
            _tribeMemberRepository = tribeMemberRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
            _storyRequestRepository = storyRequestRepository;
            _tribeRepository = tribeRepository;
        }

        public async Task<EventDTO> GetAsync(string currentUserId, int eventId)
        {
            var tribeMembers = _tribeMemberRepository.GetQueryable(true).Where(x => x.UserId == currentUserId);
            var attendees = from attendee in _eventAttendeeRepository.GetQueryable(true)
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

            var entity = await (from @event in events
                join attendee in attendees
                    on @event.Id equals attendee.EventId into gj
                from st in gj.DefaultIfEmpty()
                where @event.Id == eventId && (@event.HostId == currentUserId || st != null)
                select @event).FirstOrDefaultAsync().ConfigureAwait(false);

            var result = Mapper.Map<EventDTO>(entity);

            return result;
        }

        public async Task<ICollection<EventDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc)
        {
            var tribeMembers = _tribeMemberRepository.GetQueryable(true).Where(x => x.UserId == currentUserId);
            var attendees = _eventAttendeeRepository.GetQueryable(true);
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
            var utcNow = DateTime.UtcNow.Date;
            events = (from @event in events
                    join attendee in attendees
                        on @event.Id equals attendee.Event.Id into gj
                    from st in gj.DefaultIfEmpty()
                    where @event.CreateDateUtc < olderThanUtc && (@event.HostId == currentUserId || st != null)
                          && @event.DateUtc >= utcNow
                    orderby @event.DateUtc
                    select @event)
                .Distinct()
                .Take(20);


            var list = await events.ToListAsync().ConfigureAwait(false);
            var result = Mapper.Map<ICollection<EventDTO>>(list);

            return result;
        }

        public async Task<EventDTO> CreateAsync(string currentUserId, EventDTO newEventDTO)
        {
            _unitOfWork.BeginTransaction();
            var now = DateTime.UtcNow;
            var eventDTO = await CreateEventAsync(currentUserId, newEventDTO, now).ConfigureAwait(false);
            eventDTO.StoryRequestTitle = newEventDTO.StoryRequestTitle;
            var storyRequests =
                await CreateStoryRequests(newEventDTO.Attendees, eventDTO, now)
                    .ConfigureAwait(false);
            _unitOfWork.SaveChanges();

            var receivers = await GetNotificationReceivers(eventDTO.Id).ConfigureAwait(false);
            var eventNotifications = receivers
                .Select(receiver => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.Event,
                    RecipientId = receiver.UserId,
                    Extra = eventDTO,
                    Text = receiver.TribeId == null
                        ? $"{eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                        : $"[{receiver.TribeName}]: {eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                }).ToArray();

            var requestNotifications = receivers.Join(storyRequests, x => x.UserId, x => x.UserId,
                (receiver, requestDTO) => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.StoryRequest,
                    RecipientId = requestDTO.UserId,
                    Extra = requestDTO,
                    Text = requestDTO.TribeId == null
                        ? $"{eventDTO.HostUserName} would like you to tell a story for event \"{eventDTO.Title}\" about {eventDTO.StoryRequestTitle}"
                        : $"[{receiver.TribeName}]: {eventDTO.HostUserName} would like you to tell a story for event \"{eventDTO.Title}\" about \"{eventDTO.StoryRequestTitle}\""
                });

            await _pushNotificationsService
                .SendPushNotificationsAsync(eventNotifications.Concat(requestNotifications).ToList())
                .ConfigureAwait(false);
            return eventDTO;
        }

        public async Task<EventDTO> UpdateAsync(string currentUserId, EventDTO updateEventDTO)
        {
            _unitOfWork.BeginTransaction();
            var now = DateTime.UtcNow;
            var entity = await _eventsRepository
                .GetQueryable()
                .Include(x => x.Host)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == updateEventDTO.Id && x.HostId == currentUserId)
                .ConfigureAwait(false);
            if (entity.HostId != currentUserId)
            {
                throw new Exception("Event not found");
            }

            Mapper.Map(updateEventDTO, entity);
            await _eventsRepository.SaveAsync(entity, true).ConfigureAwait(false);
            _unitOfWork.SaveChanges();
            
            var entityId = entity.Id;
            entity = await _eventsRepository
                .GetQueryable(true)
                .Include(x => x.Host)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == entityId)
                .ConfigureAwait(false);

            var eventDTO = Mapper.Map<EventDTO>(entity);
            
            var newAttendees = updateEventDTO.Attendees.Where(x => x.Id == default(int)).ToList();
            var receivers = await GetNotificationReceivers(newAttendees).ConfigureAwait(false);
            var storyRequests = await CreateStoryRequests(newAttendees, eventDTO, now).ConfigureAwait(false);
            
            var eventNotifications = receivers
                .Select(receiver => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.Event,
                    RecipientId = receiver.UserId,
                    Extra = eventDTO,
                    Text = receiver.TribeId == null
                        ? $"{eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                        : $"[{receiver.TribeName}]: {eventDTO.HostUserName} invites you to attend an event \"{eventDTO.Title}\""
                }).ToArray();

            var requestNotifications = receivers.Join(storyRequests, x => x.UserId, x => x.UserId,
                (receiver, requestDTO) => new Notification
                {
                    Date = now,
                    Type = NotificationTypeEnum.StoryRequest,
                    RecipientId = requestDTO.UserId,
                    Extra = requestDTO,
                    Text = requestDTO.TribeId == null
                        ? $"{eventDTO.HostUserName} would like you to tell a story for event \"{eventDTO.Title}\" about {eventDTO.StoryRequestTitle}"
                        : $"[{receiver.TribeName}]: {eventDTO.HostUserName} would like you to tell a story for event \"{eventDTO.Title}\" about \"{eventDTO.StoryRequestTitle}\""
                });
            await _pushNotificationsService
                .SendPushNotificationsAsync(eventNotifications.Concat(requestNotifications).ToList())
                .ConfigureAwait(false);
            return eventDTO;
        }

        public async Task DeleteAsync(string currentUserId, int eventId)
        {
            _unitOfWork.BeginTransaction();

            var entity = await _eventsRepository
                .GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == eventId && x.HostId == currentUserId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new Exception("Comment was not found or you don't have permissions to delete it");
            }

            _eventsRepository.Remove(entity, true);
            _unitOfWork.SaveChanges();
        }

        private async Task<List<NotificationReceiver>> GetNotificationReceivers(int eventId)
        {
            var tribes = _tribeRepository.GetQueryable(true);
            var attendees = _eventAttendeeRepository.GetQueryable(true)
                .Where(x => x.EventId == eventId)
                .Select(x => new {x.TribeId, x.UserId});
            var tribeMembers = _tribeMemberRepository.GetQueryable(true);
            var receivers =
                (await (from attendee in attendees
                    join tribeMember in tribeMembers
                        on attendee.TribeId equals tribeMember.TribeId into atm
                    from tm in atm.DefaultIfEmpty()
                    join tribe in tribes on tm.TribeId equals tribe.Id into atmt
                    from x in atmt.DefaultIfEmpty()
                    select new NotificationReceiver
                    {
                        UserId = tm != null ? tm.UserId : attendee.UserId,
                        TribeId = tm != null ? tm.TribeId : (int?) null,
                        TribeName = x != null ? x.Name : null
                    }).ToListAsync().ConfigureAwait(false)).GroupBy(x => x.UserId)
                .Select(x => x.OrderBy(y => y.TribeId != null).First()).ToList();
            return receivers;
        }

        private async Task<List<NotificationReceiver>> GetNotificationReceivers(ICollection<EventAttendeeDTO> attendees)
        {
            var invitedTribes = attendees.Where(x => x.TribeId != null).Select(x => x.TribeId).ToArray();
            var tribeMembers = await _tribeRepository.GetQueryable(true).Include(x => x.Members)
                .Where(x => invitedTribes.Contains(x.Id))
                .SelectMany(x => x.Members.Select(y => new NotificationReceiver
                {
                    UserId = y.UserId,
                    TribeId = y.TribeId,
                    TribeName = x.Name
                })).ToListAsync().ConfigureAwait(false);

            var invitedUsers = attendees.Where(x => x.TribeId == null).Select(x => new NotificationReceiver
            {
                UserId = x.UserId
            }).ToArray();

            var receivers = tribeMembers.Concat(invitedUsers)
                .GroupBy(x => x.UserId)
                .Select(x => x.OrderBy(y => y.TribeId != null).First()).ToList();
            return receivers;
        }

        private async Task<EventDTO> CreateEventAsync(string currentUserId, EventDTO newEventDTO, DateTime now)
        {
            var entity = Mapper.Map<Event>(newEventDTO);
            entity.HostId = currentUserId;
            await _eventsRepository.SaveAsync(entity, true).ConfigureAwait(false);
            var entityId = entity.Id;

            entity = await _eventsRepository
                .GetQueryable(true)
                .Include(x => x.Host)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == entityId)
                .ConfigureAwait(false);

            var eventDTO = Mapper.Map<EventDTO>(entity);


            return eventDTO;
        }

        private async Task<List<StoryRequestDTO>> CreateStoryRequests(IEnumerable<EventAttendeeDTO> attendees,
            EventDTO newEventDTO, DateTime now)
        {
            var entityId = newEventDTO.Id;
            var storyRequests = attendees.Select(x => new StoryRequest
            {
                CreateDateUtc = now,
                SenderId = newEventDTO.HostId,
                UserId = x.TribeId == null ? x.UserId : null,
                TribeId = x.TribeId,
                Title = newEventDTO.StoryRequestTitle,
                EventId = entityId
            }).ToList();

            await _storyRequestRepository.AddRangeAsync(storyRequests, true).ConfigureAwait(false);

            var entityIds = storyRequests.Select(x => x.Id).ToArray();
            var requestDtos = await _storyRequestRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x => entityIds.Contains(x.Id))
                .ProjectTo<StoryRequestDTO>()
                .ToListAsync()
                .ConfigureAwait(false);
            return requestDtos;
        }
    }
}