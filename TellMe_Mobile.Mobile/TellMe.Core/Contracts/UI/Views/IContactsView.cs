using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IContactsView : IView
    {
        void DisplayContacts(ICollection<ContactDTO> contacts);
    }
}