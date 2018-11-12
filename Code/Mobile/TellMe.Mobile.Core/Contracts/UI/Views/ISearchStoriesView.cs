using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ISearchStoriesView : IView, IDismissable
    {
        event ItemsSelectedHandler<StoryListDTO> ItemsSelected;
        void ShowSuccessMessage(string message, Action complete = null);
        void DisplayStories(ICollection<StoryListDTO> items);
    }
}