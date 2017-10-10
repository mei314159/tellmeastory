using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void RequestStoryEventHandler(ICollection<StoryDTO> requestedStories);
    public interface IRequestStoryView : IView
    {
        ITextInput StoryName { get; }

        IButton SendButton { get; }

        void DisplayRecipients(ICollection<ContactDTO> selectedItems);
        void ShowSuccessMessage(string message, Action complete);
        void Close(ICollection<StoryDTO> requestedStories);
    }
}