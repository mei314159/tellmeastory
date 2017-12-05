using System;
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
    public class PlaylistsBusinessLogic : IPlaylistsBusinessLogic
    {
        private readonly List<PlaylistDTO> _events = new List<PlaylistDTO>();
        private readonly IRouter _router;
        private readonly IRemotePlaylistsDataService _remotePlaylistsDataService;
        private readonly ILocalPlaylistsDataService _localPlaylistsDataService;

        public PlaylistsBusinessLogic(IRouter router, IRemotePlaylistsDataService remotePlaylistsDataService,
            ILocalPlaylistsDataService localPlaylistsDataService)
        {
            _router = router;
            _remotePlaylistsDataService = remotePlaylistsDataService;
            _localPlaylistsDataService = localPlaylistsDataService;
        }

        public IPlaylistsView View { get; set; }

        public async Task LoadPlaylistsAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                _events.Clear();
            }

            var result = await _remotePlaylistsDataService
                .GetPlaylistsAsync(forceRefresh ? null : _events.LastOrDefault()?.CreateDateUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await _localPlaylistsDataService.SaveAllAsync(result.Data).ConfigureAwait(false);
                _events.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayItems(_events.OrderBy(x => x.CreateDateUtc).ToList());
        }

        public void CreatePlaylist()
        {
            _router.NavigateCreatePlaylist(View, EventHandler);
        }

        public void NavigateViewPlaylist(PlaylistDTO dto)
        {
            _router.NavigateViewPlaylist(View, dto, EventHandler);
        }

        private void EventHandler(PlaylistDTO item, ItemState state)
        {
            switch (state)
            {
                case ItemState.Created:
                    this._events.Insert(0, item);
                    this.View.DisplayItems(_events.OrderBy(x => x.CreateDateUtc).ToList());
                    break;
                case ItemState.Updated:
                    this.View.ReloadItem(item);
                    break;
                case ItemState.Deleted:
                    this._events.RemoveAll(x => x.Id == item.Id);
                    this.View.DisplayItems(_events.OrderBy(x => x.CreateDateUtc).ToList());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}