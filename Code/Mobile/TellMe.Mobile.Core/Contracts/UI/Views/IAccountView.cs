using System;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IAccountView : IView
    {
        UserDTO User { get; set; }
        IPicture ProfilePicture { get; }
        bool PictureChanged { get; }
        void Display();
        void ShowSuccessMessage(string message, Action complete = null);
    }
}