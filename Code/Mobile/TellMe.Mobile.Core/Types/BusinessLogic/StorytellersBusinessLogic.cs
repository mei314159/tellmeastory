using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;
using TellMe.Shared.Contracts.Enums;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class StorytellersBusinessLogic : IStorytellersBusinessLogic
    {
        private readonly IRemoteStorytellersDataService _remoteStorytellersService;
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly IRouter _router;
        private readonly EmailValidator _emailValidator;
        private readonly List<ContactDTO> _contacts = new List<ContactDTO>();

        public StorytellersBusinessLogic(IRemoteStorytellersDataService remoteStorytellersService,
            IRemoteTribesDataService remoteTribesService, IRouter router,
            EmailValidator emailValidator)
        {
            _remoteStorytellersService = remoteStorytellersService;
            _remoteTribesService = remoteTribesService;
            _router = router;
            _emailValidator = emailValidator;
        }

        public IStorytellersView View { get; set; }

        public async Task LoadAsync(bool forceRefresh, string searchText)
        {
            var result = await _remoteStorytellersService.SearchAsync(searchText, _contacts.Count, View.Mode)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                if (result.Data.Count > 0)
                {
                    _contacts.AddRange(result.Data);
                }
                else if (!string.IsNullOrWhiteSpace(searchText) && _contacts.Count == 0)
                {
                    this.View.ShowSendRequestPrompt();
                }
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayContacts(_contacts);
        }

        public async Task SendFriendshipRequestAsync(StorytellerDTO storyteller)
        {
            var result = await _remoteStorytellersService.SendFriendshipRequestAsync(storyteller.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                storyteller.FriendshipStatus = result.Data;
                this.View.ShowSuccessMessage("Follow request has been sent");
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public async Task SendRequestToJoinPromptAsync(string email)
        {
            var validationResult = _emailValidator.Validate(email);
            if (!validationResult.IsValid)
            {
                View.ShowErrorMessage("Request to join error", "Please enter correct email");
                return;
            }

            var result = await _remoteStorytellersService.SendRequestToJoinAsync(email).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Request to join has been sent");
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void AddTribe()
        {
            _router.NavigateChooseStorytellers(View, HandleStorytellerSelectedEventHandler, false,
                "Choose Tribe Membes");
        }

        private void HandleStorytellerSelectedEventHandler(IDismissable selectTribeMembersView,
            ICollection<ContactDTO> selectedContacts)
        {
            var tribeMembers = selectedContacts.Select(x => x.User).ToList();
            _router.NavigateCreateTribe(this.View, tribeMembers, TribeCreated);
        }

        public void NavigateStoryteller(StorytellerDTO storyteller)
        {
            _router.NavigateStoryteller(View, storyteller);
        }

        private void TribeCreated(TribeDTO tribe)
        {
            _contacts.Add(new ContactDTO
            {
                Tribe = tribe,
                TribeId = tribe.Id,
                Name = tribe.Name,
                Type = ContactType.Tribe
            });

            this.View.DisplayContacts(_contacts);
        }

        public async Task AcceptTribeInvitationAsync(TribeDTO dto)
        {
            var result = await _remoteTribesService.AcceptTribeInvitationAsync(dto.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.MembershipStatus = result.Data;
                this.View.DisplayContacts(_contacts);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public async Task RejectTribeInvitationAsync(TribeDTO dto)
        {
            var result = await _remoteTribesService.RejectTribeInvitationAsync(dto.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.MembershipStatus = result.Data;
                this.View.DisplayContacts(_contacts);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void ViewTribe(TribeDTO tribe)
        {
            _router.NavigateTribe(View, tribe.Id, HandleTribeLeftHandler);
        }

        private void HandleTribeLeftHandler(TribeDTO tribe)
        {
            var contact = _contacts.FirstOrDefault(x => x.TribeId == tribe.Id);
            _contacts.Remove(contact);
            View.DeleteRow(contact);
        }
    }
}