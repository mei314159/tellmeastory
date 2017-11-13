using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellerView:IView
    {
        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayStories(ICollection<StoryDTO> stories);

        void DisplayStoryteller(StorytellerDTO storyteller);
        StorytellerDTO Storyteller { get; set; }
        string StorytellerId { get; }
    }
}