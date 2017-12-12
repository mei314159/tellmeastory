using System;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ISendStoryView : IView
    {
        StoryRequestDTO StoryRequest { get; }
        NotificationDTO RequestNotification { get; }
        ContactDTO Contact { get; }
        EventDTO Event { get; set; }
        string VideoPath { get; }

        string PreviewImagePath { get; }

        ITextInput StoryTitle { get; }

        IButton SendButton { get; }
        IButton ChooseRecipientsButton { get; }

        void ShowSuccessMessage(string message, Action complete = null);
        void Close();
    }
}