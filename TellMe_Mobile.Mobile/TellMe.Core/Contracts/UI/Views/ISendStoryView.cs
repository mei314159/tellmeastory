using System;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISendStoryView : IView
    {
        StoryRequestDTO StoryRequest { get; }
        NotificationDTO RequestNotification { get; }
        ContactDTO Contact { get; }
        string VideoPath { get; }

        string PreviewImagePath { get; }

        ITextInput StoryTitle { get; }

        IButton SendButton { get; }
        IButton ChooseRecipientsButton { get; }

        void ShowSuccessMessage(string message, Action complete = null);
        void Close();
    }
}