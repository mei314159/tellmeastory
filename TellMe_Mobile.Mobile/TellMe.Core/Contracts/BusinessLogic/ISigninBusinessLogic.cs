using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ISigninBusinessLogic : IBusinessLogic
    {
        ISignInView View { get; set; }
        void Init();
        Task SignInAsync();
    }
}