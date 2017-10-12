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

namespace TellMe.Core.Types.BusinessLogic
{
    public class StorytellersBusinessLogic
    {
        private RemoteStorytellersDataService _remoteStorytellersService;
        private LocalStorytellersDataService _localStorytellersService;
        private IStorytellersView _view;
        private IRouter _router;

        public StorytellersBusinessLogic(RemoteStorytellersDataService remoteStorytellersService, IStorytellersView view, IRouter router)
        {
            _remoteStorytellersService = remoteStorytellersService;
            _localStorytellersService = new LocalStorytellersDataService();
            _view = view;
            _router = router;
        }

        public async Task LoadStorytellersAsync(bool forceRefresh = false, bool clearCache = false)
        {
            ICollection<StorytellerDTO> entities;
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

            this._view.DisplayStorytellers(entities.OrderBy(x => x.UserName).ToList());
        }

        public async Task SearchStorytellersAsync(string fragment)
        {
            var result = await _remoteStorytellersService.SearchAsync(fragment).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this._view.DisplayStorytellers(result.Data.OrderBy(x => x.UserName).ToList());
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
            }
            else
            {
                result.ShowResultError(this._view);
            }
        }
    }
}
