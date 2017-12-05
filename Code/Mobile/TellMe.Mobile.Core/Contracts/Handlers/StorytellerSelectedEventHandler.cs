using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.Handlers
{
    public delegate void StorytellerSelectedEventHandler(IDismissable view, ICollection<ContactDTO> selectedContacts);
}