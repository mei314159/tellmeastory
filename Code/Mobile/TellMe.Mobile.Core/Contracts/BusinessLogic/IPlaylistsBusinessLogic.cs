using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IPlaylistsBusinessLogic : IBusinessLogic
    {
        IPlaylistsView View { get; set; }
        Task LoadPlaylistsAsync(bool forceRefresh = false, bool clearCache = false);
        void CreatePlaylist();
        void NavigateViewPlaylist(PlaylistDTO dto);
    }
}