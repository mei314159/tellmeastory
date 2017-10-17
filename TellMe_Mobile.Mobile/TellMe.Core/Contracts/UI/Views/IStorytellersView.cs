using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void StorytellerSelectedEventHandler(StorytellerDTO selectedItem);

    public enum StorytellersViewMode
    {
        Normal,
        ChooseRecipient
    }

    public interface IStorytellersView : IView
    {
        StorytellersViewMode Mode { get; }
        event StorytellerSelectedEventHandler RecipientsSelected;

        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayStorytellers(ICollection<StorytellerDTO> items);
        void ShowSendRequestPrompt();
    }


}