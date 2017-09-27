using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.PushNotification;
using TellMe.DAL.Contracts.Services;
using TellMe.Web.DTO;

namespace TellMe.Web.Controllers
{
    [Route("api/push")]
    public class PushController : AuthorizedController
    {
        private readonly IPushNotificationsService _pushNotificationService;

        public PushController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IPushNotificationsService pushNotificationService) : base(httpContextAccessor, userService)
        {
            this._pushNotificationService = pushNotificationService;
        }

        [Authorize]
        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterToken([FromBody] PushTokenDTO dto)
        {
            await this._pushNotificationService.RegisterPushTokenAsync(dto.Token, dto.OldToken, dto.OsType, this.UserId, dto.AppVersion);
            return this.Ok();
        }
    }
}
