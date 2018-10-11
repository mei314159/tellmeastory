using System;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IStoryView : IView
    {
        void ShowSuccessMessage(string message, Action complete = null);
    }
}