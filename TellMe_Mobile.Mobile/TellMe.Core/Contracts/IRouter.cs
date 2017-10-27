using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;

namespace TellMe.Core.Contracts
{
    public interface IRouter
    {
        void NavigateMain();

        void NavigateRecordStory(IView view, StoryRequestDTO storyRequest = null, NotificationDTO notification = null);
        void NavigatePreviewStory(IView view, string videoPath, StoryRequestDTO storyRequest = null, NotificationDTO notification = null);
        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath, StoryRequestDTO storyRequest = null, NotificationDTO notification = null);

        void NavigateRequestStory(IView view, ICollection<ContactDTO> recipients);
        void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish);

        void NavigateChooseTribeMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish, HashSet<string> disabledUserIds = null);
        void NavigateViewTribe(IView view, TribeDTO tribe);

        void NavigateAccountSettings(IView view);
        void NavigateSetProfilePicture(IView view);
        void NavigateStorytellers(IView view);
        void NavigateNotificationsCenter(IView view);
        void NavigateCreateTribe(IView view, ICollection<StorytellerDTO> members, TribeCreatedHandler complete);
        void SwapToAuth();
    }
}
