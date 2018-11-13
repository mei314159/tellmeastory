using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class SearchStoriesBusinessLogic : ISearchStoriesBusinessLogic
    {
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly List<StoryListDTO> _stories = new List<StoryListDTO>();

        public SearchStoriesBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService)
        {
            _remoteStoriesDataService = remoteStoriesDataService;
        }

        public ISearchStoriesView View { get; set; }

        public async Task LoadAsync(bool forceRefresh, string searchText)
        {
            var result = await _remoteStoriesDataService.SearchAsync(searchText, _stories.Count)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                if (result.Data.Count > 0)
                {
                    _stories.Clear();
                    _stories.AddRange(result.Data);
                }
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }


            this.View.DisplayStories(_stories);
        }
    }
}