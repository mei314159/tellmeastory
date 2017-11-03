using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class StorytellersBusinessLogic
    {
        private RemoteStorytellersDataService _remoteStorytellersService;
        private LocalStorytellersDataService _localStorytellersService;
        private RemoteTribesDataService _remoteTribesService;
        private LocalTribesDataService _localTribesService;
        private IStorytellersView _view;
        private IRouter _router;
        private EmailValidator _emailValidator;
        private readonly List<ContactDTO> contacts = new List<ContactDTO>();

        public StorytellersBusinessLogic(RemoteStorytellersDataService remoteStorytellersService, RemoteTribesDataService remoteTribesService, IStorytellersView view, IRouter router)
        {
            _remoteStorytellersService = remoteStorytellersService;
            _remoteTribesService = remoteTribesService;
            _localStorytellersService = new LocalStorytellersDataService();
            _localTribesService = new LocalTribesDataService();
            _view = view;
            _router = router;
            _emailValidator = new EmailValidator();
        }

        public async Task LoadAsync(bool forceRefresh, string searchText)
        {
            bool useLocal = !string.IsNullOrWhiteSpace(searchText) && !forceRefresh;
            if (useLocal)
            {
                var storytellers = await _localStorytellersService.GetAllAsync().ConfigureAwait(false);
                useLocal = !storytellers.Expired;
                var tribes = await _localTribesService.GetAllAsync().ConfigureAwait(false);
                useLocal = !tribes.Expired;

                if (useLocal)
                {
                    contacts.Clear();
                    contacts.AddRange(storytellers.Data
                                      .Where(x =>
                                             (_view.Mode == ContactsMode.FriendsOnly && x.FriendshipStatus == FriendshipStatus.Accepted) ||
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
                    contacts.Clear();
                    await ClearLocalDB().ConfigureAwait(false);
                }

                var result = await _remoteStorytellersService.SearchAsync(searchText, contacts.Count, _view.Mode).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    if (result.Data.Count > 0)
                    {
                        contacts.AddRange(result.Data);
                        await SaveContactsToLocalDB(result).ConfigureAwait(false);
                    }
                    else if (!string.IsNullOrWhiteSpace(searchText) && contacts.Count == 0)
                    {
                        this._view.ShowSendRequestPrompt();
                    }
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }

            this._view.DisplayContacts(contacts);
        }

        private async Task ClearLocalDB()
        {
            await _localStorytellersService.DeleteAllAsync().ConfigureAwait(false);
            await _localTribesService.DeleteAllAsync().ConfigureAwait(false);
        }

        private async Task SaveContactsToLocalDB(Result<List<ContactDTO>> result)
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
            var result = await _remoteStorytellersService.SendFriendshipRequestAsync(storyteller.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                storyteller.FriendshipStatus = result.Data;
                await _localStorytellersService.SaveAsync(storyteller).ConfigureAwait(false);
                this._view.ShowSuccessMessage("Follow request has been sent");
            }
            else
            {
                result.ShowResultError(this._view);
            }
        }

        public async Task SendRequestToJoinPromptAsync(string email)
        {
            var validationResult = _emailValidator.Validate(email);
            if (!validationResult.IsValid)
            {
                _view.ShowErrorMessage("Request to join error", "Please enter correct email");
                return;
            }

            var result = await _remoteStorytellersService.SendRequestToJoinAsync(email).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this._view.ShowSuccessMessage("Request to join has been sent");
            }
            else
            {
                result.ShowResultError(this._view);
            }
        }

        public void AddTribe()
        {
            _router.NavigateChooseTribeMembers(_view, HandleStorytellerSelectedEventHandler, false);
        }

        void HandleStorytellerSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            var tribeMembers = selectedContacts.Select(x => x.User).ToList();
            _router.NavigateCreateTribe(this._view, tribeMembers, TribeCreated);
        }

        public void NavigateStoryteller(StorytellerDTO storyteller)
        {
            _router.NavigateStoryteller(_view, storyteller);
        }

        void TribeCreated(TribeDTO tribe)
        {
            contacts.Add(new ContactDTO
            {
                Tribe = tribe,
                TribeId = tribe.Id,
                Name = tribe.Name,
                Type = ContactType.Tribe
            });

            this._view.DisplayContacts(contacts);
        }

        public async Task AcceptTribeInvitationAsync(TribeDTO dto)
        {
            var result = await _remoteTribesService.AcceptTribeInvitationAsync(dto.Id, null).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.MembershipStatus = result.Data;
                await _localTribesService.SaveAsync(dto).ConfigureAwait(false);
                this._view.DisplayContacts(contacts);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }

        public async Task RejectTribeInvitationAsync(TribeDTO dto)
        {
            var result = await _remoteTribesService.RejectTribeInvitationAsync(dto.Id, null).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto.MembershipStatus = result.Data;
                await _localTribesService.SaveAsync(dto).ConfigureAwait(false);
                this._view.DisplayContacts(contacts);
            }
            else
            {
                result.ShowResultError(this._view);
                return;
            }
        }

        public void ViewTribe(TribeDTO tribe)
        {
            _router.NavigateViewTribe(_view, tribe, HandleTribeLeftHandler);
        }

        void HandleTribeLeftHandler(TribeDTO tribe)
        {
            var contact = contacts.FirstOrDefault(x => x.TribeId == tribe.Id);
            contacts.Remove(contact);
            _view.DeleteRow(contact);
        }
    }
}
