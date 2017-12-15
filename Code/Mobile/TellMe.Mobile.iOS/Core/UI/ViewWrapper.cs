using System;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Core.UI
{
    public class ViewWrapper: IView
    {
        public ViewWrapper(UIViewController controller)
        {
            Controller = controller;
        }

        public UIViewController Controller { get; }

        public void ShowErrorMessage(string title, string message = null)
        {
            this.Controller.ShowErrorMessage(title, message);
        }

        public void InvokeOnMainThread(Action action)
        {
            Controller.InvokeOnMainThread(action);
        }
    }
}
