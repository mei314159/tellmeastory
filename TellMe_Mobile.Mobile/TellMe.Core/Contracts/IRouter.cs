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
        void NavigateRecordStory(IView view);
        void NavigatePreviewStory(IView view, string videoPath);
        void NavigateRequestStory(IView view, RequestStoryEventHandler e);
        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath);
        void NavigateChooseRecipients(IView view, ContactsSelectedEventHandler e);
    }
}
