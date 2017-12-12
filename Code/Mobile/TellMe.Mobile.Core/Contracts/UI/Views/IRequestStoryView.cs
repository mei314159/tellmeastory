using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IRequestStoryView : IView
    {
        ICollection<ContactDTO> Recipients { get; }
        EventDTO Event { get; set; }
        ITextInput StoryTitle { get; }
        IButton SendButton { get; }
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(List<StoryRequestDTO> dto);
    }
}