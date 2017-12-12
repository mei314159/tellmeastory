using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ITribeViewBusinessLogic : IStoriesTableBusinessLogic
    {
        new ITribeView View { get; set; }
        void SendStory();
        void ViewStory(StoryDTO story);
        void TribeInfo();
    }
}