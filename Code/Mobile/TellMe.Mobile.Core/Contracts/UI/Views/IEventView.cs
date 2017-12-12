using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IEventView : IStoriesTableView, IDismissable
    {
        void DisplayEvent(EventDTO eventDTO, bool canEdit);
        EventDTO Event { get; set; }
        int EventId { get; }
        void EventDeleted(EventDTO eventDTO);
    }
}