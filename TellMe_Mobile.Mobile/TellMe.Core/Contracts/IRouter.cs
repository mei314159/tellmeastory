using System;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts
{
    public interface IRouter
    {
        void NavigateImportContacts();

        void NavigateMain();
        void NavigateContactDetails(ContactDTO dto);
    }
}
