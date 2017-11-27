using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellersView : IView, IDismissable
    {
        ContactsMode Mode { get; }

        event StorytellerSelectedEventHandler RecipientsSelected;

        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayContacts(ICollection<ContactDTO> contacts);
        void ShowSendRequestPrompt();
        void DeleteRow(ContactDTO contact);
    }
}