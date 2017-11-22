using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class EventsBusinessLogic : IEventsBusinessLogic
    {
        private readonly List<EventDTO> _events = new List<EventDTO>();
        private readonly IRouter _router;
        private readonly IRemoteEventsService _remoteEventsService;
        private readonly ILocalEventsService _localEventsService;

        public EventsBusinessLogic(IRouter router, IRemoteEventsService remoteEventsService, ILocalEventsService localEventsService)
        {
            _router = router;
            _remoteEventsService = remoteEventsService;
            _localEventsService = localEventsService;
        }

        public IEventsView View { get; set; }
        public async Task LoadEventsAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                _events.Clear();
            }
            
            var result = await _remoteEventsService
                .GetEventsAsync(forceRefresh ? null : _events.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localEventsService.SaveAllAsync(result.Data).ConfigureAwait(false);
                _events.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayEvents(_events.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public void CreateEvent()
        {
            _router.NavigateCreateEvent(View, HandleEventCreatedHandler);
        }

        public void NavigateViewEvent(EventDTO eventDTO)
        {
            _router.NavigateViewEvent(View, eventDTO);
        }

        public void EditEvent(EventDTO eventDTO)
        {
            _router.NavigateEditEvent(View, eventDTO);
        }

        public void NavigateStoryteller(string storytellerId)
        {
            _router.NavigateStoryteller(View, storytellerId);
        }

        void HandleEventCreatedHandler(EventDTO eventDTO)
        {

        }
    }
}