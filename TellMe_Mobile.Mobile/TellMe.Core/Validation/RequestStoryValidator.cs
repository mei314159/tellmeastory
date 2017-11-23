using FluentValidation;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Validation
{
    public class RequestStoryValidator : AbstractValidator<RequestStoryDTO>
    {
        public RequestStoryValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
        }
    }
}