using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignInView : IView
    {
        ITextInput EmailField { get; }
        ITextInput PasswordField { get; }
    }


}