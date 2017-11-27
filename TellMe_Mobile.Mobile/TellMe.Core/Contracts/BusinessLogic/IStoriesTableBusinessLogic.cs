﻿using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
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