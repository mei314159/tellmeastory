using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            this.RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            this.RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
            this.RuleFor(x => x.UserName).NotEmpty().MaximumLength(255);
        }
    }
}