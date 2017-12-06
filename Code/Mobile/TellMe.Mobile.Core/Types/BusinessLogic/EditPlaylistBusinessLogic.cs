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
        private readonly ILocalPlaylistsDataService _localPlaylistsDataService;
        private readonly PlaylistValidator _validator;
        private readonly IRouter _router;

        public EditPlaylistBusinessLogic(IRemotePlaylistsDataService remotePlaylistsDataService, IRouter router,
            ILocalPlaylistsDataService localPlaylistsDataService, PlaylistValidator validator)
        {
            _localPlaylistsDataService = localPlaylistsDataService;
            _validator = validator;
            _router = router;
            _remotePlaylistsDataService = remotePlaylistsDataService;
        }

        public ICreatePlaylistView View { get; set; }

        public async Task LoadAsync(bool forceRefresh)
        {
            var dto = View.Playlist;
            var localEventResult = await _localPlaylistsDataService.GetAsync(dto.Id).ConfigureAwait(false);
            if (forceRefresh || localEventResult.Expired || localEventResult.Data.Stories == null ||
                localEventResult.Data.Stories.Count == 0)
            {
                var result = await _remotePlaylistsDataService.GetAsync(dto.Id).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localPlaylistsDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    dto = result.Data;
                }
                else
                {
                    result.ShowResultError(this.View);
                    return;
                }
            }
            else
            {
                dto = localEventResult.Data;
            }

            this.View.Display(dto);
        }

        public void ChooseStories()
        {
            var disabledStories =
                new HashSet<int>(View.Playlist.Stories.Select(x => x.Id));
            _router.NavigateSearchStories(View, StoriesSelectedEventHandler, true, disabledStories);
        }

        private void StoriesSelectedEventHandler(IDismissable selectAttendeesView, ICollection<StoryListDTO> stories)
        {
            View.Playlist.Stories.AddRange(stories);
            View.DisplayStories();
        }

        public async Task SaveAsync()
        {
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
                    await _localPlaylistsDataService.SaveAsync(result.Data).ConfigureAwait(false);
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
                await _localPlaylistsDataService.DeleteAsync(View.Playlist).ConfigureAwait(false);
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