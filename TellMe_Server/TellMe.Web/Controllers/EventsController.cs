using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.Services;
using TellMe.Web.Extensions;

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
        
        [HttpGet("older-than/{olderThanTicksUtc}")]
        public async Task<IActionResult> GetEventsAsync(long olderThanTicksUtc)
        {
            var olderThanUtc = olderThanTicksUtc.GetUtcDateTime();
            var result = await _eventService.GetAllAsync(this.UserId, olderThanUtc);

            return Ok(result);
        }
        
        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] EventDTO dto)
        {
            var result = await _eventService.CreateAsync(this.UserId, dto);
            return Ok(result);
        }
    }
}