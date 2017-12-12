using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IPlaylistsView : IView
    {
        void DisplayItems(ICollection<PlaylistDTO> items);
        void ReloadItem(PlaylistDTO dto);
    }
}