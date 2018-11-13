using System.Collections.Generic;
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
    public class EventViewBusinessLogic : StoriesTableBusinessLogic, IEventViewBusinessLogic
    {
        private readonly ILocalAccountService _localLocalAccountService;
        private readonly IRemoteEventsDataService _remoteEventsDataService;
        private readonly List<StoryDTO> _stories = new List<StoryDTO>();


        public EventViewBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, IRouter router,
            IRemoteEventsDataService remoteEventsDataService,
            ILocalAccountService localLocalAccountService,
            IRemoteStorytellersDataService remoteStorytellersDataService) : base(remoteStoriesDataService, router,
            localLocalAccountService, remoteStorytellersDataService)
        {
            _remoteEventsDataService = remoteEventsDataService;
            _localLocalAccountService = localLocalAccountService;
        }

        public new IEventView View
        {
            get => (IEventView) base.View;
            set => base.View = value;
        }

        public override async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                LoadEventAsync(View.Event?.Id ?? View.EventId, true);
                _stories.Clear();
            }

            var result = await RemoteStoriesDataService
                .GetEventStoriesAsync(View.Event.Id, forceRefresh ? null : _stories.LastOrDefault()?.CreateDateUtc)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                _stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayStories(_stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public override async Task<bool> InitAsync()
        {
            if (View.Event == null)
            {
                var result = await LoadEventAsync(View.EventId).ConfigureAwait(false);
                if (!result)
                    return false;
            }

            var currentUserId = _localLocalAccountService.GetAuthInfo().Account.Id;
            View.DisplayEvent(View.Event, View.Event.HostId == currentUserId);
            return true;
        }

        private async Task<bool> LoadEventAsync(int eventId, bool forceRefresh = false)
        {
            var result = await _remoteEventsDataService.GetEventAsync(eventId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                View.Event = result.Data;
                return true;
            }

            result.ShowResultError(this.View);
            return false;
        }

        public void SendStory()
        {
            /*Router.NavigateRecordStory(View, contact: new ContactDTO
            {
                Type = ContactType.Tribe,
                TribeId = View.Event.Id,
                Tribe = View.Event
            });*/
        }

        public void ViewStory(StoryDTO story)
        {
            Router.NavigateViewStory(this.View, story);
        }

        public async Task DeleteEvent()
        {
            var result = await _remoteEventsDataService
                .DeleteEventAsync(View.Event.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("You've deleted this event", HandleEventDeleted);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void EditEvent()
        {
            Router.NavigateEditEvent(View, View.Event, EventStateChanged);
        }

        private void EventStateChanged(EventDTO item, ItemState state)
        {
            if (state == ItemState.Deleted)
            {
                this.View.EventDeleted(item);
            }
        }

        private void HandleEventDeleted()
        {
            View.EventDeleted(View.Event);
        }
    }
}