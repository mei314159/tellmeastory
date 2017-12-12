using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class RequestStoryValidator : AbstractValidator<RequestStoryDTO>
    {
        public RequestStoryValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
        }
    }
}