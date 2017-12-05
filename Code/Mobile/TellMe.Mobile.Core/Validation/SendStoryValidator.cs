using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class SendStoryValidator : AbstractValidator<StoryDTO>
    {
        public SendStoryValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
        }
    }
}