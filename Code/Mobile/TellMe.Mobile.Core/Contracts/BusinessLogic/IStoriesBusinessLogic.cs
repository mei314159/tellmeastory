using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStoriesBusinessLogic : IStoriesTableBusinessLogic
    {
        new IStoriesListView View { get; set; }
        void SendStory();
        void RequestStory();
        void NotificationsCenter();
        Task LoadActiveNotificationsCountAsync();
    }
}