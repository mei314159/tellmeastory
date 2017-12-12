using System;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ICreateEventView : IView, IDismissable
    {
        EventDTO Event { get; set; }

        void Display(EventDTO eventDTO);
        void DisplayMembers();
        void ShowSuccessMessage(string message, Action complete = null);
        void Deleted(EventDTO deletedEventDTO);
        void Saved(EventDTO eventDTO);
        IOverlay DisableInput();
        void EnableInput(IOverlay overlay);
    }
}