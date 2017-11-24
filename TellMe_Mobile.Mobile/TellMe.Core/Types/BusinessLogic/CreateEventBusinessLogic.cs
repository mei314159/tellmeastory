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
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class CreateEventBusinessLogic : ICreateEventBusinessLogic
    {
        private readonly IRemoteEventsDataService _remoteEventsDataService;
        private readonly ILocalEventsDataService _localEventsDataService;
        private readonly CreateEventValidator _validator;
        private readonly IRouter _router;

        public CreateEventBusinessLogic(IRemoteEventsDataService remoteEventsDataService, IRouter router,
            ILocalEventsDataService localEventsDataService, CreateEventValidator validator)
        {
            _localEventsDataService = localEventsDataService;
            _validator = validator;
            _router = router;
            _remoteEventsDataService = remoteEventsDataService;
        }

        public ICreateEventView View { get; set; }

        public async Task LoadAsync(bool forceRefresh)
        {
            var eventDTO = View.Event;
            var tribeResult = await _localEventsDataService.GetAsync(eventDTO.Id).ConfigureAwait(false);
            if (forceRefresh || tribeResult.Expired || tribeResult.Data.Attendees == null ||
                tribeResult.Data.Attendees.Count == 0)
            {
                var result = await _remoteEventsDataService.GetEventAsync(eventDTO.Id).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localEventsDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    eventDTO = result.Data;
                }
                else
                {
                    result.ShowResultError(this.View);
                    return;
                }
            }
            else
            {
                eventDTO = tribeResult.Data;
            }

            this.View.Display(eventDTO);
        }

        public void ChooseMembers()
        {
            var disabledUserIds =
                new HashSet<string>(View.Event.Attendees.Where(x => x.UserId != null).Select(x => x.UserId));
            var disabledTribeIds =
                new HashSet<int>(View.Event.Attendees.Where(x => x.TribeId != null).Select(x => x.TribeId.Value));
            _router.NavigateChooseEventMembers(View, AttendeesSelectedEventHandler, true, disabledUserIds,
                disabledTribeIds);
        }

        private void AttendeesSelectedEventHandler(IDismissable selectAttendeesView,
            ICollection<ContactDTO> selectedContacts)
        {
            foreach (var contact in selectedContacts)
            {
                View.Event.Attendees.Add(new EventAttendeeDTO
                {
                    UserId = contact.TribeId == null ? contact.User.Id : null,
                    AttendeeName = contact.TribeId == null ? contact.User.UserName : contact.Tribe.Name,
                    AttendeePictureUrl = contact.TribeId == null ? contact.User.PictureUrl : null,
                    AttendeeFullName = contact.TribeId == null ? contact.User.FullName : null,
                    TribeId = contact.TribeId,
                    EventId = View.Event.Id,
                    Status = EventAttendeeStatus.Pending
                });
            }

            View.DisplayMembers();
        }

        public void NavigateCreateRequest()
        {
            var validationResult = _validator.Validate(View.Event);
            if (validationResult.IsValid)
            {
                _router.NavigatePrepareStoryRequest(this.View, View.Event.Attendees.Select(x => new ContactDTO
                {
                    Name = x.AttendeeName,
                    TribeId = x.TribeId,
                    UserId = x.UserId,
                    Type = x.TribeId.HasValue ? ContactType.Tribe : ContactType.User
                }).ToArray(), StoryRequestCreated);
            }
            else
            {
                validationResult.ShowValidationResult(this.View);
            }
        }

        private async void StoryRequestCreated(RequestStoryDTO dto, ICollection<ContactDTO> recipients)
        {
            var overlay = this.View.DisableInput();
            this.View.Event.StoryRequestTitle = dto.Title;
            var result = await _remoteEventsDataService
                .SaveEventAsync(View.Event)
                .ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                await _localEventsDataService.SaveAsync(result.Data).ConfigureAwait(false);
                this.View.ShowSuccessMessage("Event successfully saved", () => this.View.Dismiss());
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void NavigateAttendee(EventAttendeeDTO eventAttendeeDTO)
        {
            if (eventAttendeeDTO.TribeId == null)
                _router.NavigateStoryteller(View, eventAttendeeDTO.UserId);
            else
                _router.NavigateTribe(View, eventAttendeeDTO.TribeId.Value, TribeLeftHandler);
        }

        private void TribeLeftHandler(TribeDTO tribe)
        {
            View.Event.Attendees.RemoveAll(x => x.TribeId == tribe.Id);
            this.View.DisplayMembers();
        }

        public async Task DeleteEventAsync()
        {
            var result = await _remoteEventsDataService
                .DeleteEventAsync(View.Event.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localEventsDataService.DeleteAsync(View.Event).ConfigureAwait(false);
                this.View.ShowSuccessMessage($"You've deleted the event \"{View.Event.Title}\"",
                    () => View.Close(View.Event));
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}