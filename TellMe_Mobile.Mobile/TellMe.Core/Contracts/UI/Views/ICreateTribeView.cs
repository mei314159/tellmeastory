using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ICreateTribeView : IView
    {
        ICollection<StorytellerDTO> Members { get; }

        string TribeName { get; }

        void Close(TribeDTO tribe);
        void ShowSuccessMessage(string message, Action complete = null);
    }
}