using FluentValidation;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Validation
{
    public class SignInValidator : AbstractValidator<ISignInView>
    {
        public SignInValidator()
        {
            this.RuleFor(x => x.EmailField.Text).NotEmpty().EmailAddress().MaximumLength(255);
            this.RuleFor(x => x.PasswordField.Text).NotEmpty().MaximumLength(255);
        }
    }
}