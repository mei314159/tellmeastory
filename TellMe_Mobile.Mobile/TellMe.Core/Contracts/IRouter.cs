﻿using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts
{
    public interface IRouter
    {
        void NavigateMain();

        void NavigateRecordStory(IView view, StoryRequestDTO storyRequest = null, NotificationDTO notification = null,
            ContactDTO contact = null);

        void NavigatePreviewStory(IView view, string videoPath, StoryRequestDTO storyRequest = null,
            NotificationDTO notification = null, ContactDTO contact = null);

        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath,
            StoryRequestDTO storyRequest = null, NotificationDTO notification = null, ContactDTO contact = null);

        void NavigatePrepareStoryRequest(IView view, ICollection<ContactDTO> recipients, StoryRequestCreatedEventHandler e);
        void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish);

        void NavigateChooseTribeMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish,
            HashSet<string> disabledUserIds = null);

        void NavigateChooseEventMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish,
            HashSet<string> disabledUserIds = null,
            HashSet<int> disabledTribeIds = null);

        void NavigateAccountSettings(IView view);
        void NavigateSetProfilePicture(IView view);
        void NavigateStorytellers(IView view);
        void NavigateStoryteller(IView view, StorytellerDTO storyteller);
        void NavigateStoryteller(IView view, string userId);
        void NavigateNotificationsCenter(IView view);
        void NavigateCreateTribe(IView view, ICollection<StorytellerDTO> members, TribeCreatedHandler complete);
        void NavigateViewStory(IView view, StoryDTO story, bool goToComments = false);
        void NavigateTribeInfo(IView view, TribeDTO tribe, TribeLeftHandler e);
        void NavigateTribe(IView view, TribeDTO tribe, TribeLeftHandler e);
        void NavigateTribe(IView view, int tribeId, TribeLeftHandler e);
        void NavigateViewEvent(IView view, EventDTO eventDTO, EventDeletedHandler eventDeleted);
        void NavigateCreateEvent(IView view, EventSavedHandler eventSaved, EventDeletedHandler eventDeleted);
        void NavigateEditEvent(IView view, EventDTO eventDTO, EventSavedHandler eventSaved, EventDeletedHandler eventDeleted);
        void NavigateEvents(IView view);
        void SwapToAuth();
    }
}