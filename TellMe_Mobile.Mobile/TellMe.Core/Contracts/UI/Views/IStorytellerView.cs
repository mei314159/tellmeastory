using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellerView : IStoriesListView
    {
        void DisplayStoryteller(StorytellerDTO storyteller);
        StorytellerDTO Storyteller { get; set; }
        string StorytellerId { get; }
    }
}