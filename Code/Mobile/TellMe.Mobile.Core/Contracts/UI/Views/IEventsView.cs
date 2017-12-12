using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IEventsView : IView
    {
        void DisplayItems(ICollection<EventDTO> items);
        void ReloadItem(EventDTO dto);
    }
}