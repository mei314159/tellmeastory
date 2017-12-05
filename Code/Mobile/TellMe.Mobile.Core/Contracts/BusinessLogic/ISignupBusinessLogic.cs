using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface ISignupBusinessLogic : IBusinessLogic
    {
        ISignUpView View { get; set; }
        void Init();
        Task SignUpAsync();
    }
}