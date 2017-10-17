using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void RequestStoryEventHandler(ICollection<StoryDTO> requestedStories);
    public interface IRequestStoryView : IView
    {
        StorytellerDTO Recipient { get; }
        ITextInput StoryTitle { get; }
        IButton SendButton { get; }
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(StoryDTO requestedStory);
    }
}