using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ISigninBusinessLogic : IBusinessLogic
    {
        ISignInView View { get; set; }
        void Init();
        Task SignInAsync();
    }
}