using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellerView : IStoriesTableView
    {
		StorytellerDTO Storyteller { get; set; }
		string StorytellerId { get; }
        void DisplayStoryteller(StorytellerDTO storyteller);
    }
}