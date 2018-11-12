using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IEditPlaylistBusinessLogic : IBusinessLogic
    {
        ICreatePlaylistView View { get; set; }
        void NavigateStory(int storyId);
        void ChooseStories();
        Task LoadAsync(bool forceRefresh);
        Task DeletePlaylistAsync();
        Task SaveAsync();
        Task<List<StoryDTO>> LoadStoriesAsync(int playlistId);
    }
}