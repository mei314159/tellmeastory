using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStoriesBusinessLogic : IStoriesTableBusinessLogic
    {
        new IStoriesListView View { get; set; }
        void SendStory();
        void RequestStory();
        void AccountSettings();
        void ShowStorytellers();
        void NotificationsCenter();
        Task LoadActiveNotificationsCountAsync();
        void NavigateEvents();
        void NavigatePlaylists();
    }
}