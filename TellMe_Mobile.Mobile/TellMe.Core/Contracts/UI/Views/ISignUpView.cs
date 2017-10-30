using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignUpView : ISignInView
    {
        ITextInput UserNameField { get; }
        ITextInput FullNameField { get; }
        ITextInput ConfirmPasswordField { get; }
    }
}