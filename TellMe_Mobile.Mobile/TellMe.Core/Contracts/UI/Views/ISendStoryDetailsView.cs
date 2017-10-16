using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISendStoryDetailsView : IView
    {
        StoryDTO RequestedStory { get; }

        string VideoPath { get; }

        string PreviewImagePath { get; }

        ITextInput StoryName { get; }

        IButton SendButton { get; }
        IButton ChooseRecipientsButton { get; }

        void DisplayRecipients(ICollection<StorytellerDTO> selectedItems);
        void ShowSuccessMessage(string message, Action complete);
        void Close();
    }
}