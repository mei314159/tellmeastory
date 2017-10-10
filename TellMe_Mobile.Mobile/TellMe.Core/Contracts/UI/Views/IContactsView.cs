using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
	public delegate void ContactsSelectedEventHandler(ICollection<ContactDTO> selectedItems);

    public interface IContactsView : IView
    {
		event ContactsSelectedEventHandler ContactsSelected;
        void DisplayContacts(ICollection<ContactDTO> contacts);
        void SelectCell(object cell, bool selected);
        void Done(List<ContactDTO> selectedItems);
    }
}