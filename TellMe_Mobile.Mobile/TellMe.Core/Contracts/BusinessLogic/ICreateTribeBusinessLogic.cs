using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ICreateTribeBusinessLogic
    {
        Task CreateTribeAsync();
        ICreateTribeView View { get; set; }
    }
}