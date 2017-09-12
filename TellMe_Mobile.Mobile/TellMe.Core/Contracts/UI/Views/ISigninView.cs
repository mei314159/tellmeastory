using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISigninView
    {
        ITextInput EmailField { get; }

        ITextInput PasswordField { get; }
    }
}
