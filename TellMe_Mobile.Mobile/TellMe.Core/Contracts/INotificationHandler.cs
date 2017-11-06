using System;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.DTO;

namespace TellMe.Core.Contracts
{
    public interface INotificationHandler
    {
        void ProcessNotification(NotificationDTO notification, IView view, Action<int, bool> complete = null);
    }
}