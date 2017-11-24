using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellerView : IView
    {
		StorytellerDTO Storyteller { get; set; }
		string StorytellerId { get; }
        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayStories(ICollection<StoryDTO> stories);

        void DisplayStoryteller(StorytellerDTO storyteller);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
    }
}