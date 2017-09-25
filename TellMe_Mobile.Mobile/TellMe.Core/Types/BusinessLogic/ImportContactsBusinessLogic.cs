using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.Providers;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.Core.Types.BusinessLogic
{
    public class ImportContactsBusinessLogic
    {
        private IContactsProvider _contactsProvider;
        private IImportContactsView _view;
        private ContactsService _contactsService;
        private IRouter _router;
        public ImportContactsBusinessLogic(IRouter _router, IContactsProvider _contactsProvider, ContactsService _contactsService, IImportContactsView _view)
        {
			this._router = _router;
            this._contactsProvider = _contactsProvider;
			this._contactsService = _contactsService;
            this._view = _view;
        }

        public async Task SynchronizeContacts()
        {
            var status = _contactsProvider.GetPermissions();
            if (status == Permissions.Denied
            || status == Permissions.Restricted)
            {
                _view.ShowErrorMessage("Access denied",
                        "Please provide an access to your contacts list manually in Settings > Privacy > Contacts");
                return;
            }

            var contacts = _contactsProvider.GetContacts();
            var syncResult = await _contactsService.SynchronizeContactsAsync(contacts).ConfigureAwait(false);
            if (syncResult.IsValid)
            {
                _router.NavigateMain();
            }
            else
            {
                _view.ShowErrorMessage("Error", syncResult.ErrorsString);
            }
        }
    }
}
