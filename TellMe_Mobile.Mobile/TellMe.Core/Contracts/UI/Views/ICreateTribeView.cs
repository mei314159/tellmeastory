using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void TribeCreatedHandler(TribeDTO tribe);
    public interface ICreateTribeView : IView
    {
        ICollection<StorytellerDTO> Members { get; }

        string TribeName { get; }

        void Close(TribeDTO tribe);
        void ShowSuccessMessage(string message, Action complete = null);
    }
}