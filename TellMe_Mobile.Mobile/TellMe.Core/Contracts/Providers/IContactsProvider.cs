using System.Collections.Generic;
using TellMe.Core.DTO;

namespace TellMe.Core.Contracts.Providers
{
    public interface IContactsProvider
    {
        IReadOnlyCollection<PhoneContactDTO> GetContacts();

        Permissions GetPermissions();
    }
}
