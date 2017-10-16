using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void UserSelectedEventHandler(ICollection<StorytellerDTO> selectedItems);

    public interface IContactsView : IView
    {
		event UserSelectedEventHandler ContactsSelected;
        void DisplayContacts(ICollection<StorytellerDTO> contacts);
        void SelectCell(object cell, bool selected);
        void Done(List<StorytellerDTO> selectedItems);
    }
}