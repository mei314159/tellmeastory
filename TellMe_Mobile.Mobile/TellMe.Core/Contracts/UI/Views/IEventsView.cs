using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IEventsView : IView
    {
        void DisplayEvents(ICollection<EventDTO> events);
    }
}