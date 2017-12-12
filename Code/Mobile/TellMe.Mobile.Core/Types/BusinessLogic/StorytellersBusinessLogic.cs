using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class StorytellersBusinessLogic : IStorytellersBusinessLogic
    {
        private readonly IRemoteStorytellersDataService _remoteStorytellersService;
        private readonly ILocalStorytellersDataService _localStorytellersService;
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly ILocalTribesDataService _localTribesService;
        private readonly IRouter _router;
        private readonly EmailValidator _emailValidator;
        private readonly List<ContactDTO> _contacts = new List<ContactDTO>();

        public StorytellersBusinessLogic(IRemoteStorytellersDataService remoteStorytellersService,
            IRemoteTribesDataService remoteTribesService, IRouter router,
            ILocalStorytellersDataService localStorytellersService, ILocalTribesDataService localTribesService,
            EmailValidator emailValidator)
        {
            _remoteStorytellersService = remoteStorytellersService;
            _remoteTribesService = remoteTribesService;
            _router = router;
            _localStorytellersService = localStorytellersService;
            _localTribesService = localTribesService;
            _emailValidator = emailValidator;
        }

        public IStorytellersView View { get; set; }

        public async Task LoadAsync(bool forceRefresh, string searchText)
        {
            var useLocal = !string.IsNullOrWhiteSpace(searchText) && !forceRefresh;
            if (useLocal)
            {
                var storytellers = await _localStorytellersService.GetAllAsync().ConfigureAwait(false);
                useLocal = !storytellers.Expired;

                DataResult<ICollection<TribeDTO>> tribes = null;
                if (useLocal)
                {
                    tribes = await _localTribesService.GetAllAsync().ConfigureAwait(false);
                    useLocal = !tribes.Expired;
                }

                if (useLocal)
                {
                    _contacts.Clear();
                    _contacts.AddRange(storytellers.Data
                        .Where(x =>
                            (View.Mode == ContactsMode.ChooseTribeMembers &&
                             x.FriendshipStatus == FriendshipStatus.Accepted) ||
                            x.FriendshipStatus != FriendshipStatus.None
                        ).Select(x => new ContactDTO
                        {
                            Type = ContactType.User,
                            UserId = x.Id,
                            User = x
                        }).Union(tribes.Data.Select(x => new ContactDTO
                        {
                            Type = ContactType.Tribe,
                            TribeId = x.Id,
                            Tribe = x
                        })));
                }
            }

            if (!useLocal)
            {
                if (forceRefresh)
                {
                    _contacts.Clear();
                    await ClearLocalDb().ConfigureAwait(false);
                }

                var result = await _remoteStorytellersService.SearchAsync(searchText, _contacts.Count, View.Mode)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    if (result.Data.Count > 0)
                    {
                        _contacts.AddRange(result.Data);
                        await SaveContactsToLocalDb(result).ConfigureAwait(false);
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
            }

            this.View.DisplayContacts(_contacts);
        }

        private async Task ClearLocalDb()
        {
            await _localStorytellersService.DeleteAllAsync().ConfigureAwait(false);
            await _localTribesService.DeleteAllAsync().ConfigureAwait(false);
        }

        private async Task SaveContactsToLocalDb(Result<List<ContactDTO>> result)
        {
            await _localStorytellersService
                .SaveAllAsync(result.Data
                    .Where(x =>
                        x.Type == ContactType.User &&
                        x.User.FriendshipStatus != FriendshipStatus.None)
                    .Select(x => x.User))
                .ConfigureAwait(false);
            await _localTribesService
                .SaveAllAsync(result.Data.Where(x => x.Type == ContactType.Tribe)
                    .Select(x => x.Tribe))
                .ConfigureAwait(false);
        }

        public async Task SendFriendshipRequestAsync(StorytellerDTO storyteller)
        {
            var result = await _remoteStorytellersService.SendFriendshipRequestAsync(storyteller.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                storyteller.FriendshipStatus = result.Data;
                await _localStorytellersService.SaveAsync(storyteller).ConfigureAwait(false);
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
            _router.NavigateChooseTribeMembers(View, HandleStorytellerSelectedEventHandler, false);
        }

        private void HandleStorytellerSelectedEventHandler(IDismissable selectTribeMembersView, ICollection<ContactDTO> selectedContacts)
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
                await _localTribesService.SaveAsync(dto).ConfigureAwait(false);
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
                await _localTribesService.SaveAsync(dto).ConfigureAwait(false);
                this.View.DisplayContacts(_contacts);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void ViewTribe(TribeDTO tribe)
        {
            _router.NavigateTribe(View, tribe, HandleTribeLeftHandler);
        }

        private void HandleTribeLeftHandler(TribeDTO tribe)
        {
            var contact = _contacts.FirstOrDefault(x => x.TribeId == tribe.Id);
            _contacts.Remove(contact);
            View.DeleteRow(contact);
        }
    }
}