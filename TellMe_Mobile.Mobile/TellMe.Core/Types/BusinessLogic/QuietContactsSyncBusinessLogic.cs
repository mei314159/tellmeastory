using System.Threading.Tasks;
using TellMe.Core.Contracts.Providers;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.Core.Types.BusinessLogic
{
    public class QuietContactsSyncBusinessLogic
    {
        private IContactsProvider _contactsProvider;
        private RemoteContactsDataService _contactsService;
        public QuietContactsSyncBusinessLogic(IContactsProvider _contactsProvider, RemoteContactsDataService _contactsService)
        {
            this._contactsProvider = _contactsProvider;
            this._contactsService = _contactsService;
        }

        public async Task SynchronizeContacts()
        {
            var status = _contactsProvider.GetPermissions();
            if (status == Permissions.Authorized)
            {
                var contacts = _contactsProvider.GetContacts();
                await _contactsService.SynchronizeContactsAsync(contacts).ConfigureAwait(false);
            }
        }
    }
}
