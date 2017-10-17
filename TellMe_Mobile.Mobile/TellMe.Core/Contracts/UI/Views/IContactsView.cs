using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IContactsView : IView
    {
		event StorytellerSelectedEventHandler ContactsSelected;
        void DisplayContacts(ICollection<StorytellerDTO> contacts);
        void SelectCell(object cell, bool selected);
        void Done(List<StorytellerDTO> selectedItems);
    }
}