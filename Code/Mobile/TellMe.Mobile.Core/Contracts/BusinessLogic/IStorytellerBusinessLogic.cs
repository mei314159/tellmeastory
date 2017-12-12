using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStorytellerBusinessLogic : IStoriesTableBusinessLogic
    {
        new IStorytellerView View { get; set; }
        void SendStory();
        void RequestStory();
    }
}