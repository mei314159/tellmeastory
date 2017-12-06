using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class SearchStoriesBusinessLogic : ISearchStoriesBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly ILocalStoriesDataService _localStoriesDataService;
        private readonly List<StoryListDTO> _stories = new List<StoryListDTO>();

        public SearchStoriesBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, ILocalStoriesDataService localStoriesDataService)
        {
            _remoteStoriesDataService = remoteStoriesDataService;
            _localStoriesDataService = localStoriesDataService;
        }

        public ISearchStoriesView View { get; set; }

        public async Task LoadAsync(bool forceRefresh, string searchText)
        {
                var useLocal = !string.IsNullOrWhiteSpace(searchText) && !forceRefresh;
            if (useLocal)
            {
                var stories = await _localStoriesDataService.GetAllAsync().ConfigureAwait(false);
                useLocal = !stories.Expired;

                if (useLocal)
                {
                    _stories.Clear();
                    _stories.AddRange(stories.Data.Select(x=>new StoryListDTO
                    {
                        CreateDateUtc = x.CreateDateUtc,
                        Id = x.Id,
                        PreviewUrl = x.PreviewUrl,
                        SenderId = x.SenderId,
                        SenderName = x.SenderName,
                        SenderPictureUrl = x.SenderPictureUrl,
                        Title = x.Title
                    }));
                }
            }

            if (!useLocal)
            {
                if (forceRefresh)
                {
                    _stories.Clear();
                }

                var result = await _remoteStoriesDataService.SearchAsync(searchText, _stories.Count)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    if (result.Data.Count > 0)
                    {
                        _stories.AddRange(result.Data);
                    }
                }
                else
                {
                    result.ShowResultError(this.View);
                    return;
                }
            }

            this.View.DisplayStories(_stories);
        }
    }
}