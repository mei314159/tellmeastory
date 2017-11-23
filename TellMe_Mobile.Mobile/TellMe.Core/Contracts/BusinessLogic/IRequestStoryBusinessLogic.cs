using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IRequestStoryBusinessLogic : IBusinessLogic
    {
        IRequestStoryView View { get; set; }
        Task CreateStoryRequest();
        string GetUsername();
    }
}