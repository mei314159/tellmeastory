using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IEventViewBusinessLogic : IStoriesTableBusinessLogic
    {
        new IEventView View { get; set; }
        void SendStory();
        void ViewStory(StoryDTO story);
        Task DeleteEvent();
        void EditEvent();
    }
}