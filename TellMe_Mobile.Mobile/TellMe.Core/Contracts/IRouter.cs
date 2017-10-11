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
        void NavigateRecordStory(IView view, StoryDTO requestedStory = null);
        void NavigatePreviewStory(IView view, string videoPath, StoryDTO requestedStory = null);
        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath, StoryDTO requestedStory = null);

		void NavigateRequestStory(IView view, RequestStoryEventHandler e);
        void NavigateChooseRecipients(IView view, ContactsSelectedEventHandler e);
        void NavigateAccountSettings(IStoriesListView view);
    }
}
