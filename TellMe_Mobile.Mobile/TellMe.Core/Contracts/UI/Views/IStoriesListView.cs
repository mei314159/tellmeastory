using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStoriesListView : IView
    {
        void ShowSuccessMessage(string message);

        void DisplayStories(ICollection<StoryDTO> stories);
    }
}