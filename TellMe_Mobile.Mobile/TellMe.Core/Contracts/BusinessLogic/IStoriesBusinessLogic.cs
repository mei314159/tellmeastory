using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IStoriesBusinessLogic : IBusinessLogic
    {
        IStoriesListView View { get; set; }
        Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false);
        void SendStory();
        void RequestStory();
        void AccountSettings();
        void ShowStorytellers();
        void NotificationsCenter();
        void ViewStory(StoryDTO story, bool goToComments = false);
        void NavigateStoryteller(StoryDTO story);
        void ViewReceiver(StoryReceiverDTO receiver, TribeLeftHandler onRemoveTribe);
        Task LoadActiveNotificationsCountAsync();
        Task LikeButtonTouchedAsync(StoryDTO story);
    }
}