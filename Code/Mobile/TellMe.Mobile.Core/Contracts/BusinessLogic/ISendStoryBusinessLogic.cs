using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ISendStoryBusinessLogic : IBusinessLogic
    {
        ISendStoryView View { get; set; }
        void Init();
        void InitButtons();
        Task SendAsync();
        void ChooseRecipients();
        string GetUsername();
    }
}