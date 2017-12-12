﻿using System;
using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Shared.Contracts.DTO;
using StorytellerDTO = TellMe.Mobile.Core.Contracts.DTO.StorytellerDTO;

namespace TellMe.Mobile.Core.Contracts
{
    public interface IRouter
    {
        void NavigateMain();

        void NavigateRecordStory(IView view, StoryRequestDTO storyRequest = null, NotificationDTO notification = null,
            ContactDTO contact = null, EventDTO eventDTO = null);

        void NavigatePreviewStory(IView view, string videoPath, StoryRequestDTO storyRequest = null,
            NotificationDTO notification = null, ContactDTO contact = null, EventDTO eventDTO = null);

        void NavigateStoryDetails(IView view, string videoPath, string previewImagePath,
            StoryRequestDTO storyRequest = null, NotificationDTO notification = null, ContactDTO contact = null, EventDTO eventDTO = null);

        void NavigatePrepareStoryRequest(IView view, ICollection<ContactDTO> recipients, ItemUpdateHandler<List<StoryRequestDTO>> e, EventDTO eventDTO = null);
        void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish);

        void NavigateChooseTribeMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish,
            HashSet<string> disabledUserIds = null);

        void NavigateAccountSettings(IView view);
        void NavigateSetProfilePicture(IView view);
        void NavigateStorytellers(IView view);
        void NavigateStoryteller(IView view, StorytellerDTO storyteller);
        void NavigateStoryteller(IView view, string userId);
        void NavigateNotificationsCenter(IView view);
        void NavigateCreateTribe(IView view, ICollection<StorytellerDTO> members, TribeCreatedHandler complete);
        void NavigateViewStory(IView view, StoryDTO story, bool goToComments = false);
        void NavigateViewStory(IView view, int storyId, bool goToComments = false);
        void NavigateTribeInfo(IView view, TribeDTO tribe, TribeLeftHandler e);
        void NavigateTribe(IView view, TribeDTO tribe, TribeLeftHandler e);
        void NavigateTribe(IView view, int tribeId, TribeLeftHandler e);
        void NavigateViewEvent(IView view, EventDTO eventDTO, ItemUpdateHandler<EventDTO> eventStateChanged);
        void NavigateCreateEvent(IView view, ItemUpdateHandler<EventDTO> eventStateChanged);
        void NavigateEditEvent(IView view, EventDTO eventDTO, ItemUpdateHandler<EventDTO> eventStateChanged);
        void NavigateEvents(IView view);
        void NavigatePlaylists(IView view);
        void NavigateCreatePlaylist(IView view, ItemUpdateHandler<PlaylistDTO> eventHandler);
        void NavigateViewPlaylist(IView view, PlaylistDTO dto, ItemUpdateHandler<PlaylistDTO> eventHandler);
        void SwapToAuth();
        void NavigateSearchStories(IView view, ItemsSelectedHandler<StoryListDTO> storiesSelectedEventHandler, bool dismissOnFinish, HashSet<int> disabledStories);
    }
}