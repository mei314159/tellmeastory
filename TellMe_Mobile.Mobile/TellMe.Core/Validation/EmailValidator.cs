using FluentValidation;

namespace TellMe.Core.Validation
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            this.RuleFor(x => x).NotEmpty().EmailAddress().MaximumLength(255);
        }
    }
}