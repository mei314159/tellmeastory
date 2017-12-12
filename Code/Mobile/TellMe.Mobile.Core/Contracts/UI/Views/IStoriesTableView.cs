using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IStoriesTableView : IView
    {
        void ShowSuccessMessage(string message, Action complete = null);
        void DisplayStories(ICollection<StoryDTO> stories);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
    }
}