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
        private IStorytellersView _view;
        private IRouter _router;
        private EmailValidator _emailValidator;
        public StorytellersBusinessLogic(RemoteStorytellersDataService remoteStorytellersService, IStorytellersView view, IRouter router)
        {
            _remoteStorytellersService = remoteStorytellersService;
            _localStorytellersService = new LocalStorytellersDataService();
            _view = view;
            _router = router;
            _emailValidator = new EmailValidator();
        }

        public async Task LoadStorytellersAsync(bool forceRefresh = false, bool clearCache = false)
        {
            IEnumerable<StorytellerDTO> entities;
            var localEntities = await _localStorytellersService.GetAllAsync().ConfigureAwait(false);
            if (localEntities.Expired || forceRefresh || clearCache)
            {
                if (clearCache)
                {
                    await _localStorytellersService.DeleteAllAsync().ConfigureAwait(false);
                }
                var result = await _remoteStorytellersService.GetAllAsync().ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localStorytellersService.SaveAllAsync(result.Data).ConfigureAwait(false);
                    entities = result.Data;
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }
            else
            {
                entities = localEntities.Data;
            }


            if (_view.Mode == StorytellersViewMode.ChooseRecipient)
            {
                entities = entities.Where(x => x.FriendshipStatus == FriendshipStatus.Accepted);
            }

            this._view.DisplayStorytellers(entities.OrderBy(x => x.UserName).ToList());
        }

        public async Task SearchStorytellersAsync(string fragment)
        {
            var result = await _remoteStorytellersService.SearchAsync(fragment).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                if (result.Data.Count > 0)
                {
                    this._view.DisplayStorytellers(result.Data.OrderBy(x => x.UserName).ToList());
                }
                else
                {
                    this._view.ShowSendRequestPrompt();
                }
            }
            else
            {
                result.ShowResultError(this._view);
            }
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
    }
}
