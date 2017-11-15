using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ISendStoryBusinessLogic : IBusinessLogic
    {
        ISendStoryView View { get; set; }
        void Init();
        void InitButtons();
        Task SendAsync();
        void ChooseRecipients();
    }
}