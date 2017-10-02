using FluentValidation;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Validation
{
    public class SignUpPhoneValidator : AbstractValidator<ISignUpPhoneView>
    {
        public SignUpPhoneValidator()
        {
            this.RuleFor(x => x.CountryCodeField.Text).Matches("^\\+\\d{1,}$");
        }
    }
}