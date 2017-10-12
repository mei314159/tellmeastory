using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class ContactsBusinessLogic
    {
        private RemoteContactsDataService _remoteContactsService;
        private LocalContactsDataService _localContactsService;
        private IContactsView _view;
        private IRouter _router;
        private List<ContactDTO> selectedItems;

        public ContactsBusinessLogic(IRouter router, RemoteContactsDataService contactsService, IContactsView view)
        {
            _router = router;
            _remoteContactsService = contactsService;
            _localContactsService = new LocalContactsDataService();
            _view = view;
            selectedItems = new List<ContactDTO>();
        }

        public async Task LoadContactsAsync(bool forceRefresh = false)
        {
            ICollection<ContactDTO> contacts;
            var localContacts = await _localContactsService.GetAllAsync().ConfigureAwait(false);
            if (localContacts.Expired || forceRefresh)
            {
                var result = await _remoteContactsService.GetContactsAsync();
                if (result.IsSuccess)
                {
                    await _localContactsService.SaveContactsAsync(result.Data).ConfigureAwait(false);
                    contacts = result.Data;
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }
            else
            {
                contacts = localContacts.Data;
            }

            this._view.DisplayContacts(contacts);
        }

        public void GoToImportContacts()
        {
        }

        public void ContactSelected(ContactDTO dto, object cell)
        {
            if (dto.IsAppUser)
            {
                var selected = selectedItems.Contains(dto);
                if (selected)
                    selectedItems.Remove(dto);
                else
                    selectedItems.Add(dto);
                
                this._view.SelectCell(cell, !selected);
                //this._router.NavigateContactDetails(this._view, dto);
            }
        }

        public void DoneButtonTouched()
        {
            _view.Done(this.selectedItems);
        }
    }
}
