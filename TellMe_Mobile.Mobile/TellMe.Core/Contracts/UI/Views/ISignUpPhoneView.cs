using FluentValidation.Results;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ISignUpPhoneView
    {
        ITextInput PhoneNumberField { get; }

        string ConfirmationCode { get; }

        void ShowErrorMessage(string title, string message = null);
    }
}