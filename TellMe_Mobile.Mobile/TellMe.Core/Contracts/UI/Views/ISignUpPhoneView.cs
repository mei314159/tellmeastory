using FluentValidation.Results;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignUpPhoneView : IView
    {
        ITextInput PhoneNumberField { get; }

        string ConfirmationCode { get; }
    }
}