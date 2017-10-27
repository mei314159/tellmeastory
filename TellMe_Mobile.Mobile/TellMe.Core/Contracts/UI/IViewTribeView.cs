using System;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.UI
{
    public delegate void TribeLeftHandler(TribeDTO tribe);
    public interface IViewTribeView : IView
    {
        TribeDTO Tribe { get; set; }

        void Display(TribeDTO tribe);
        void DisplayMembers();
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(TribeDTO tribeLeft);
    }
}
