using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface ISignupBusinessLogic : IBusinessLogic
    {
        ISignUpView View { get; set; }
        void Init();
        Task SignUpAsync();
    }
}