using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class PlaylistViewBusinessLogic : IPlaylistViewBusinessLogic
    {
        private readonly IRouter _router;
        private readonly ILocalAccountService _localAccountService;
        private readonly IRemotePlaylistsDataService _remotePlaylistsDataService;

        public PlaylistViewBusinessLogic(IRouter router, IRemotePlaylistsDataService remotePlaylistsDataService, ILocalAccountService localAccountService)
        {
            _router = router;
            _remotePlaylistsDataService = remotePlaylistsDataService;
            _localAccountService = localAccountService;
        }

        public IPlaylistView View { get; set; }


        public bool CanSaveOrder => View.Playlist.AuthorId == _localAccountService.GetAuthInfo().UserId;

        public void Share()
        {
            _router.NavigateChooseStorytellers(this.View, StorytellersSelected, true, "Share with");
        }

        private async void StorytellersSelected(IDismissable view, ICollection<ContactDTO> selectedContacts)
        {
            var overlay = this.View.DisableInput();
            var dto = new SharePlaylistDTO
            {
                PlaylistId = View.Playlist.Id,
                UserIds = selectedContacts.Select(x => x.UserId).ToList()
            };

            var result = await _remotePlaylistsDataService
                .ShareAsync(dto)
                .ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Playlist successfully shared");
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public async Task SaveAsync()
        {
            var overlay = this.View.DisableInput();
            var dto = new PlaylistDTO
            {
                Id = View.Playlist.Id,
                Name = View.Playlist.Name,
                Stories = View.Playlist.Stories.Select((x, i) => new StoryListDTO
                {
                    Id = x.Id,
                    Order = i
                }).ToList()
            };

            var result = await _remotePlaylistsDataService
                .SaveAsync(dto)
                .ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Playlist successfully saved", View.PlaylistSaved);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}