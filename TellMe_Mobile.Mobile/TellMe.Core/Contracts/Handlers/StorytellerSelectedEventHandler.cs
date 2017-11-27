using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.Handlers
{
    public delegate void StorytellerSelectedEventHandler(IDismissable view, ICollection<ContactDTO> selectedContacts);
}