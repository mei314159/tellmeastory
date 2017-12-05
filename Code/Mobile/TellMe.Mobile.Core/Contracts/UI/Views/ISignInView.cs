using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface ISignInView : IView
    {
        ITextInput EmailField { get; }
        ITextInput PasswordField { get; }
    }
}