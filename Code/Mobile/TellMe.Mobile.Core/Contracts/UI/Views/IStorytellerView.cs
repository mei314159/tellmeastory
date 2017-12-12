using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IStorytellerView : IStoriesTableView
    {
        StorytellerDTO Storyteller { get; set; }
		string StorytellerId { get; }
        void DisplayStoryteller(StorytellerDTO storyteller);
    }
}