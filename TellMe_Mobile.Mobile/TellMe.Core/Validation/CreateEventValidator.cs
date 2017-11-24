using System;
using System.Linq;
using FluentValidation;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Validation
{
    public class CreateEventValidator : AbstractValidator<EventDTO>
    {
        public CreateEventValidator()
        {
            this.RuleFor(x => x.Title).NotEmpty()
                .WithMessage("Please enter the title of event")
                .MaximumLength(255);
            this.RuleFor(x => x.Description).NotEmpty()
                .WithMessage("Please enter the description of event")
                .MaximumLength(255);
            this.RuleFor(x => x.DateUtc).GreaterThan(DateTime.UtcNow)
                .WithMessage("The date of event must be later than now");
            this.RuleFor(x => x.Attendees).Must(x => x?.Any() == true)
                .WithMessage("You should invite at least one attendee to your event.");
        }
    }
}