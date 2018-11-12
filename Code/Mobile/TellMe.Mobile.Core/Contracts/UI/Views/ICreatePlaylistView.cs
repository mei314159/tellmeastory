using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Shared.Contracts.DTO.Interfaces;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ICreatePlaylistView : IView, IDismissable
    {
        PlaylistDTO Playlist { get; set; }
        List<IStoryDTO> Stories { get; set; }

        void Display(PlaylistDTO dto);
        void DisplayStories();
        void ShowSuccessMessage(string message, Action complete = null);
        void Deleted(PlaylistDTO dto);
        void Saved(PlaylistDTO dto);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
    }
}