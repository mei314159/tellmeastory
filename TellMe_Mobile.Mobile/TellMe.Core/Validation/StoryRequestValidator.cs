using FluentValidation;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Validation
{
    public class StoryRequestValidator : AbstractValidator<StoryRequestDTO>
    {
        public StoryRequestValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
        }
    }
}