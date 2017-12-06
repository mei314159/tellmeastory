using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ISearchStoriesBusinessLogic : IBusinessLogic
    {
        ISearchStoriesView View { get; set; }
        Task LoadAsync(bool forceRefresh, string searchText);
    }


}