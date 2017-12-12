﻿using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStoriesTableBusinessLogic : IBusinessLogic
    {
        IStoriesTableView View { get; set; }
        Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false);
        void ViewStory(StoryDTO story, bool goToComments = false);
        void NavigateStoryteller(string userId);
        void NavigateTribe(int tribeId, TribeLeftHandler onRemoveTribe);
        void NavigateReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe);
        Task LikeButtonTouchedAsync(StoryDTO story);
        Task<bool> InitAsync();
    }
}