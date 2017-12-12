using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Types.Services
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
            _unitOfWork.SaveChanges();
            return eventDTO;
        }

        public async Task<EventDTO> UpdateAsync(string currentUserId, EventDTO updateEventDTO)
        {
            _unitOfWork.BeginTransaction();
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
        
        private async Task<EventDTO> CreateEventAsync(string currentUserId, EventDTO newEventDTO, DateTime now)
        {
            var entity = Mapper.Map<Event>(newEventDTO);
            entity.HostId = currentUserId;
            entity.Attendees = new List<EventAttendee>
            {
                new EventAttendee
                {
                    CreateDateUtc = DateTime.UtcNow,
                    Status = EventAttendeeStatus.Host,
                    UserId = currentUserId
                }
            };
            
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
    }
}