using System.Linq;
using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class CreateTribeValidator : AbstractValidator<TribeDTO>
    {
        public CreateTribeValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Please enter a tribe name")
                .MaximumLength(255);
            this.RuleFor(x => x.Members).Must(x => x?.Any() == true)
                .WithMessage("Tribe should have at least one member.");
        }
    }
}