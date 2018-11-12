using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IPlaylistView : IView
    {
        PlaylistDTO Playlist { get; }
        List<StoryDTO> Stories { get; set; }
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
        
        void ShowSuccessMessage(string message, Action complete = null);
        void PlaylistSaved();
    }
}