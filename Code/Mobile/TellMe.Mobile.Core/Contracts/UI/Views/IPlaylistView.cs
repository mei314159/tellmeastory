using System;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IPlaylistView : IView
    {
        PlaylistDTO Playlist { get; }
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
        
        void ShowSuccessMessage(string message, Action complete = null);
        void PlaylistSaved();
    }
}