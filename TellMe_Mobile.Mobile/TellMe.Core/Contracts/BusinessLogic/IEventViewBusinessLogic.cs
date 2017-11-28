using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
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