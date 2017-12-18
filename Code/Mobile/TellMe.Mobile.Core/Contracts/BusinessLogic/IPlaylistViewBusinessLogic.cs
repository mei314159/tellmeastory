using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IPlaylistViewBusinessLogic : IBusinessLogic
    {
        IPlaylistView View { get; set; }

        void Share();
        
        Task SaveAsync();
    }
}