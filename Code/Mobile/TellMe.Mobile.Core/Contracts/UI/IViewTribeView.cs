using System;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.UI
{
    public interface IViewTribeView : IView
    {
        TribeDTO Tribe { get; set; }

        void Display(TribeDTO tribe);
        void DisplayMembers();
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(TribeDTO tribeLeft);
    }
}