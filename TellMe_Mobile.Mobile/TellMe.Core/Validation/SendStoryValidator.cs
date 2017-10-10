using FluentValidation;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Validation
{
    public class SendStoryValidator : AbstractValidator<StoryDTO>
    {
        public SendStoryValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
        }
    }
}