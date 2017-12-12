using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class EditEventBusinessLogic : IEditEventBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly IRemoteEventsDataService _remoteEventsDataService;
        private readonly ILocalEventsDataService _localEventsDataService;
        private readonly EventValidator _validator;
        private readonly IRouter _router;

        public EditEventBusinessLogic(IRemoteEventsDataService remoteEventsDataService, IRouter router,
            ILocalEventsDataService localEventsDataService, EventValidator validator,
            IRemoteStoriesDataService remoteStoriesDataService)
        {
            _localEventsDataService = localEventsDataService;
            _validator = validator;
            _remoteStoriesDataService = remoteStoriesDataService;
            _router = router;
            _remoteEventsDataService = remoteEventsDataService;
        }

        public ICreateEventView View { get; set; }

        public async Task LoadAsync(bool forceRefresh)
        {
            var eventDTO = View.Event;
            var localEventResult = await _localEventsDataService.GetAsync(eventDTO.Id).ConfigureAwait(false);
            if (forceRefresh || localEventResult.Expired || localEventResult.Data.Attendees == null ||
                localEventResult.Data.Attendees.Count == 0)
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
                eventDTO = localEventResult.Data;
            }

            this.View.Display(eventDTO);
        }

        public void NavigateCreateRequest()
        {
            _router.NavigateChooseRecipients(View, RequestStoryRecipientSelectedEventHandler, false);
        }

        public async Task SaveAsync()
        {
            var createMode = View.CreateMode;
            var validationResult = _validator.Validate(View.Event);
            if (validationResult.IsValid)
            {
                var overlay = this.View.DisableInput();
                var result = await _remoteEventsDataService
                    .SaveEventAsync(View.Event)
                    .ConfigureAwait(false);
                this.View.EnableInput(overlay);
                if (result.IsSuccess)
                {
                    await _localEventsDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    this.View.Event = result.Data;
                    if (createMode)
                        this.View.PromptCreateRequest(result.Data);
                    else
                    {
                        this.View.ShowSuccessMessage("Event successfully saved");   
                    }
                }
                else
                {
                    result.ShowResultError(this.View);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this.View);
            }
        }

        public void NavigateAttendee(EventAttendeeDTO eventAttendeeDTO)
        {
            if (eventAttendeeDTO.TribeId == null)
                _router.NavigateStoryteller(View, eventAttendeeDTO.UserId);
            else
                _router.NavigateTribe(View, eventAttendeeDTO.TribeId.Value, TribeLeftHandler);
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
                    () => View.Deleted(View.Event));
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        private void TribeLeftHandler(TribeDTO tribe)
        {
            View.Event.Attendees.RemoveAll(x => x.TribeId == tribe.Id);
            this.View.DisplayMembers();
        }

        private void RequestStoryRecipientSelectedEventHandler(IDismissable chooseRecipientsView,
            ICollection<ContactDTO> selectedContacts)
        {
            _router.NavigatePrepareStoryRequest(this.View, selectedContacts, (item, state) => chooseRecipientsView.Dismiss(), View.Event);
        }
    }
}