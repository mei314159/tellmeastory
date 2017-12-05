using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ICreateTribeBusinessLogic : IBusinessLogic
    {
        Task CreateTribeAsync();
        ICreateTribeView View { get; set; }
    }
}