using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ITribeViewBusinessLogic : IBusinessLogic
    {
        ITribeView View { get; set; }
        Task LoadStoriesAsync(bool forceRefresh = false);
        Task<bool> InitAsync();
        void SendStory();
        void ViewStory(StoryDTO story);
        void TribeInfo();
        void NavigateStoryteller(StoryDTO story);
        void ViewReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe);
        Task LikeButtonTouchedAsync(StoryDTO story);
    }
}