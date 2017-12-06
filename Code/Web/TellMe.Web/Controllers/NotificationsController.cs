using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.Extensions;

namespace TellMe.Web.Controllers
{
    [Route("api/notifications")]
    public class NotificationsController : AuthorizedController
    {
        INotificationService _notificationService;
        public NotificationsController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            INotificationService notificationService) : base(httpContextAccessor, userService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("older-than/{olderThanTicksUtc}")]
        public async Task<IActionResult> GetNotificationsAsync(long olderThanTicksUtc)
        {
            var olderThanUtc = olderThanTicksUtc.GetUtcDateTime();
            var result = await _notificationService.GetNotificationsAsync(this.UserId, olderThanUtc);

            return Ok(result);
        }

        [HttpGet("active/count")]
        public async Task<IActionResult> GetActiveNotificationsCountAsync(long olderThanTicksUtc)
        {
            var result = await _notificationService.GetActiveNotificationsCountAsync(this.UserId);
            return Ok(result);
        }

        [HttpPost("{id}/handled")]
        public async Task<IActionResult> HandleAsync(int id)
        {
            await _notificationService.HandleNotificationAsync(this.UserId, id);
            return Ok();
        }
    }
}
