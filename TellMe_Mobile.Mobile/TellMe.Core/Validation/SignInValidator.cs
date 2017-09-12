using FluentValidation;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Validation
{
    public class SignInValidator : AbstractValidator<ISigninView>
    {
        public SignInValidator()
        {
            this.RuleFor(x => x.EmailField.Text).EmailAddress().WithName("Email");
            this.RuleFor(x => x.PasswordField.Text).MinimumLength(6).WithName("Password");
        }
    }
}