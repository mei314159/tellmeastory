using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IEventView : IStoriesTableView
    {
        void DisplayEvent(EventDTO eventDTO);
        EventDTO Event { get; set; }
        int EventId { get; }
        void EventDeleted(EventDTO eventDTO);
    }
}