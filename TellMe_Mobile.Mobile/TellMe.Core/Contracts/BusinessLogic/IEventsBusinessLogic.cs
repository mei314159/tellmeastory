using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
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