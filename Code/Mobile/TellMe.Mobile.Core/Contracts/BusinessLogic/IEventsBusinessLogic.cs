using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IEventsBusinessLogic : IBusinessLogic
    {
        IEventsView View { get; set; }
        Task LoadEventsAsync(bool forceRefresh = false, bool clearCache = false);
        void CreateEvent();
        void NavigateViewEvent(EventDTO eventDTO);
        void EditEvent(EventDTO eventDTO);
        void NavigateStoryteller(string storytellerId);
        void NavigateTribe(int tribeId, TribeLeftHandler onRemoveTribe);
    }
}