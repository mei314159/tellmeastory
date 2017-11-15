using FluentValidation;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Validation
{
    public class SignUpValidator : AbstractValidator<ISignUpView>
    {
        public SignUpValidator()
        {
            this.RuleFor(x => x.EmailField.Text).NotEmpty().EmailAddress().MaximumLength(255);
            this.RuleFor(x => x.FullNameField.Text).NotEmpty().MaximumLength(255);
            this.RuleFor(x => x.PasswordField.Text).NotEmpty().MaximumLength(255);
            this.RuleFor(x => x.ConfirmPasswordField.Text).NotEmpty().Equal(x => x.PasswordField.Text);
        }
    }
}