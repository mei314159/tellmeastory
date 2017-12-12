using System;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IView
    {
        void ShowErrorMessage(string title, string message = null);

        void InvokeOnMainThread(Action action);
    }
}