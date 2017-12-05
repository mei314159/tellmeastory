using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.Providers
{
    public interface IContactsProvider
    {
        IReadOnlyCollection<PhoneContactDTO> GetContacts();

        Permissions GetPermissions();
    }
}