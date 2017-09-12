using System;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignupView: ISigninView
    {
        ITextInput ConfirmPasswordField { get; }
    }
}
