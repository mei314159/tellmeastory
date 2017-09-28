using System;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts
{
    public interface IRouter
    {
        void NavigateImportContacts();

        void NavigateMain();
        void NavigateContactDetails(IView view, ContactDTO dto);
    }
}
