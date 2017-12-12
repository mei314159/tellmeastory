namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IStoriesListView : IStoriesTableView
    {
        void DisplayNotificationsCount(int count);
    }
}