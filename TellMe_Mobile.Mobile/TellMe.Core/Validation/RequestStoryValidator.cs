using FluentValidation;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Validation
{
    public class RequestStoryValidator : AbstractValidator<RequestStoryDTO>
    {
        public RequestStoryValidator()
        {
            this.RuleForEach(x => x.Requests).Must(x => !string.IsNullOrWhiteSpace(x.Title)).WithMessage("Story title is required.");
        }
    }
}