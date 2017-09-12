using System;
using FluentValidation;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Validation
{
    public class SignUpValidator : AbstractValidator<ISignupView>
    {
        public SignUpValidator()
		{
            this.RuleFor(x => x.EmailField.Text).EmailAddress().WithName("Email");
            this.RuleFor(x => x.PasswordField.Text).MinimumLength(6).Matches("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$").WithName("Password");
            this.RuleFor(x => x.ConfirmPasswordField.Text)
                .Must((ISignupView view, string val) =>
                      string.Equals(view.PasswordField.Text, val, StringComparison.InvariantCulture))
                .WithMessage("'Password' and 'Confirmation password' fields must be equal.");
        }
    }
}