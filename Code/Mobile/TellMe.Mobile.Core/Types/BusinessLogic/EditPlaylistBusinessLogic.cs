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
using TellMe.Mobile.Core.Validation;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class EditPlaylistBusinessLogic : IEditPlaylistBusinessLogic
    {
        private readonly IRemotePlaylistsDataService _remotePlaylistsDataService;
        private readonly PlaylistValidator _validator;
        private readonly IRouter _router;

        public EditPlaylistBusinessLogic(IRemotePlaylistsDataService remotePlaylistsDataService, IRouter router, PlaylistValidator validator)
        {
            _validator = validator;
            _router = router;
            _remotePlaylistsDataService = remotePlaylistsDataService;
        }

        public ICreatePlaylistView View { get; set; }

        public async Task LoadAsync(bool forceRefresh)
        {
            var dto = View.Playlist;
            var result = await _remotePlaylistsDataService.GetAsync(dto.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                dto = result.Data;
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.Display(dto);
        }

        public void ChooseStories()
        {
            var disabledStories = new HashSet<int>(View.Stories.Select(x => x.Id));
            _router.NavigateSearchStories(View, StoriesSelectedEventHandler, true, disabledStories);
        }

        private void StoriesSelectedEventHandler(IDismissable selectAttendeesView, ICollection<StoryListDTO> stories)
        {
            View.Stories.AddRange(stories);
            View.DisplayStories();
        }

        public async Task SaveAsync()
        {
            View.Playlist.Stories = View.Stories.Select((x, i) => new StoryOrderDTO
            {
                Id = x.Id,
                Order = i
            }).ToList();

            var validationResult = _validator.Validate(View.Playlist);
            if (validationResult.IsValid)
            {
                var overlay = this.View.DisableInput();
                var result = await _remotePlaylistsDataService
                    .SaveAsync(View.Playlist)
                    .ConfigureAwait(false);
                this.View.EnableInput(overlay);
                if (result.IsSuccess)
                {
                    this.View.ShowSuccessMessage("Playlist successfully saved", () => this.View.Saved(result.Data));
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

        public async Task<List<StoryDTO>> LoadStoriesAsync(int playlistId)
        {
            var result = await this._remotePlaylistsDataService.GetStoriesAsync(playlistId).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                result.ShowResultError(this.View);
            }

            return result.Data;
        }

        public void NavigateStory(int storyId)
        {
            _router.NavigateViewStory(View, storyId);
        }

        public async Task DeletePlaylistAsync()
        {
            var result = await _remotePlaylistsDataService
                .DeleteAsync(View.Playlist.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage($"You've deleted a playlist \"{View.Playlist.Name}\"",
                    () => View.Deleted(View.Playlist));
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}