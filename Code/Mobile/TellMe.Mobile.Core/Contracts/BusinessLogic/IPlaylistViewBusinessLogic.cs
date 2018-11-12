using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IPlaylistViewBusinessLogic : IBusinessLogic
    {
        IPlaylistView View { get; set; }
        
        bool CanSaveOrder { get; }

        void Share();
        
        Task SaveAsync();
        Task<List<StoryDTO>> LoadStoriesAsync(int playlistId);
    }
}