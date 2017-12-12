using System.Linq;
using FluentValidation;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class RequestStoryValidator : AbstractValidator<RequestStoryDTO>
    {
        public RequestStoryValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty().WithMessage("Story title is required.");
            
            this.RuleFor(x => x.Recipients).Must(x => x?.Any() == true)
                .WithMessage("You should select at least one recipient.");
        }
    }
}