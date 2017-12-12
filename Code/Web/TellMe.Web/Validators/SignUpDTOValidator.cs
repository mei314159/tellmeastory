using FluentValidation;
using Microsoft.AspNetCore.Identity;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DTO;

namespace TellMe.Web.Validators
{
    public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
    {
        public SignUpDTOValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.UserName)
            .NotEmpty()
            .MustAsync(async (x, t) => (await userManager.FindByNameAsync(x).ConfigureAwait(false)) == null)
            .WithMessage("Username is already in use");

            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (x, t) => (await userManager.FindByEmailAsync(x).ConfigureAwait(false)) == null)
            .WithMessage("The user with the same email is already exist");

            RuleFor(x => x.Password).NotEmpty();

            RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);
        }
    }
}