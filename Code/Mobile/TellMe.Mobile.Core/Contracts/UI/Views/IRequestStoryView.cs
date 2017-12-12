using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IRequestStoryView : IView
    {
        ICollection<ContactDTO> Recipients { get; }
        ITextInput StoryTitle { get; }
        IButton SendButton { get; }
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(RequestStoryDTO dto, ICollection<ContactDTO> recipients);
    }
}