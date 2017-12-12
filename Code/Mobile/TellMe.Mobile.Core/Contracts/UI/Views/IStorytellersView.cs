using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using ContactDTO = TellMe.Mobile.Core.Contracts.DTO.ContactDTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
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