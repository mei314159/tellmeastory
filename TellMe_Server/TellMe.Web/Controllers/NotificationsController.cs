using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;

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

        [HttpGet("skip/{skip}")]
        public async Task<IActionResult> GetNotificationsAsync(int skip)
        {
            var result = await _notificationService.GetNotificationsAsync(this.UserId, skip < 0 ? 0 : skip);

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
