using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void StorytellerSelectedEventHandler(IDismissable view, ICollection<ContactDTO> selectedContacts);
}