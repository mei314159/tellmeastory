using System;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ICreateEventView : IView, IDismissable
    {
        EventDTO Event { get; set; }
        bool CreateMode { get; }
        void Display(EventDTO eventDTO);
        void DisplayMembers();
        void ShowSuccessMessage(string message, Action complete = null);
        void Deleted(EventDTO deletedEventDTO);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
        void PromptCreateRequest(EventDTO eventDTO);
    }
}