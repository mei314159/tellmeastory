using System;
using FluentValidation;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Validation
{
    public class EventValidator : AbstractValidator<EventDTO>
    {
        public EventValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty()
                .WithMessage("Please enter the title of event")
                .MaximumLength(255);
            this.RuleFor(x => x.Description).NotEmpty()
                .WithMessage("Please enter the description of event")
                .MaximumLength(255);
            this.RuleFor(x => x.DateUtc).GreaterThan(DateTime.UtcNow)
                .WithMessage("The date of event must be later than now");
        }
    }
}