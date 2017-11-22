using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{
    [Route("api/events")]
    public class EventsController:AuthorizedController
    {
        private readonly IEventService _eventService;
        public EventsController(IHttpContextAccessor httpContextAccessor, IUserService userService, IEventService eventService) : base(httpContextAccessor, userService)
        {
            _eventService = eventService;
        }
    }
}