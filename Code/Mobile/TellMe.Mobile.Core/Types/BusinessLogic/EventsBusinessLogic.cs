﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class EventsBusinessLogic : IEventsBusinessLogic
    {
        private readonly List<EventDTO> _events = new List<EventDTO>();
        private readonly IRouter _router;
        private readonly IRemoteEventsDataService _remoteEventsDataService;
        private readonly ILocalEventsDataService _localEventsDataService;

        public EventsBusinessLogic(IRouter router, IRemoteEventsDataService remoteEventsDataService,
            ILocalEventsDataService localEventsDataService)
        {
            _router = router;
            _remoteEventsDataService = remoteEventsDataService;
            _localEventsDataService = localEventsDataService;
        }

        public IEventsView View { get; set; }

        public async Task LoadEventsAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                _events.Clear();
            }

            var result = await _remoteEventsDataService
                .GetEventsAsync(forceRefresh ? null : _events.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localEventsDataService.SaveAllAsync(result.Data).ConfigureAwait(false);
                _events.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayItems(_events.OrderBy(x => x.DateUtc).ToList());
        }

        public void CreateEvent()
        {
            _router.NavigateCreateEvent(View, EventCreated, EventDeleted);
        }

        public void NavigateViewEvent(EventDTO eventDTO)
        {
            _router.NavigateViewEvent(View, eventDTO, EventDeleted);
        }

        public void EditEvent(EventDTO eventDTO)
        {
            _router.NavigateEditEvent(View, eventDTO, EventUpdated, EventDeleted);
        }

        public void NavigateStoryteller(string storytellerId)
        {
            _router.NavigateStoryteller(View, storytellerId);
        }

        public void NavigateTribe(int tribeId, TribeLeftHandler onRemoveTribe)
        {
            _router.NavigateTribe(this.View, tribeId, onRemoveTribe);
        }

        private void EventCreated(EventDTO eventDTO)
        {
            this._events.Insert(0, eventDTO);
            this.View.DisplayItems(_events.OrderBy(x => x.DateUtc).ToList());
        }

        private void EventUpdated(EventDTO eventDTO)
        {
            this.View.ReloadItem(eventDTO);
        }

        private void EventDeleted(EventDTO eventDTO)
        {
            this._events.RemoveAll(x => x.Id == eventDTO.Id);
            this.View.DisplayItems(_events.OrderBy(x => x.DateUtc).ToList());
        }
    }
}