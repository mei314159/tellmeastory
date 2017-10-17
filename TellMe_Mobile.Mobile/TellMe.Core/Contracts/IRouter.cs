using System;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts
{
    public interface IRouter
    {
        void NavigateMain();

        void NavigateRecordStory(IView view, StoryDTO requestedStory = null, NotificationDTO notification = null);
        void NavigatePreviewStory(IView view, string videoPath, StoryDTO requestedStory = null, NotificationDTO notification = null);
        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath, StoryDTO requestedStory = null, NotificationDTO notification = null);

        void NavigateRequestStory(IView view, StorytellerDTO recipient);
        void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish);
        void NavigateAccountSettings(IView view);
        void NavigateSetProfilePicture(IView view);
        void NavigateStorytellers(IView view);
        void NavigateNotificationsCenter(IView view);
    }
}
