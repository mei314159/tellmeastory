using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.Handlers
{
    public delegate void ItemsSelectedHandler<T>(IDismissable view, ICollection<T> item);
}