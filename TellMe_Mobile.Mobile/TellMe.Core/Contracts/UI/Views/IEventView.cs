using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IEventView : IStoriesTableView, IDismissable
    {
        void DisplayEvent(EventDTO eventDTO, bool canEdit);
        EventDTO Event { get; set; }
        int EventId { get; }
        void EventDeleted(EventDTO eventDTO);
    }
}