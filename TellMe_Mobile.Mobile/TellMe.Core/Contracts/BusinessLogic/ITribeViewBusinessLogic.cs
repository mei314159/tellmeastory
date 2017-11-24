using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ITribeViewBusinessLogic : IStoriesTableBusinessLogic
    {
        new ITribeView View { get; set; }
        void SendStory();
        void ViewStory(StoryDTO story);
        void TribeInfo();
    }
}