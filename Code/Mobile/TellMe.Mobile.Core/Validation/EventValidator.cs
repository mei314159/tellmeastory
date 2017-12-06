using System;
using System.Linq;
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
            this.RuleFor(x => x.Attendees).Must(x => x?.Any() == true)
                .WithMessage("You should invite at least one attendee to your event.");
        }
    }
    
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