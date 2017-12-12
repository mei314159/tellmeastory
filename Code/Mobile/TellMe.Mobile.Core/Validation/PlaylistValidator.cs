using System.Linq;
using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class PlaylistValidator : AbstractValidator<PlaylistDTO>
    {
        public PlaylistValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Please enter the title of playlist");
            this.RuleFor(x => x.Stories).Must(x => x?.Any() == true)
                .WithMessage("You should add at least one story to your playlist.");
        }
    }
}