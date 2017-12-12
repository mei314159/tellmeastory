using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IRequestStoryBusinessLogic : IBusinessLogic
    {
        IRequestStoryView View { get; set; }
        Task CreateStoryRequest();
        string GetUsername();
    }
}