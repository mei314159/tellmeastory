using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStoriesListView : IView
    {
        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayStories(ICollection<StoryDTO> stories);
        void DisplayNotificationsCount(int count);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
    }
}