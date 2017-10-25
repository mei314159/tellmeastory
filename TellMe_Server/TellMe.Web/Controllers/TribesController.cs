using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{
    [Route("api/tribes")]
    public class TribesController : AuthorizedController
    {
        private ITribeService _tribeService;
        private INotificationService _notificationService;
        public TribesController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ITribeService tribeService,
            INotificationService notificationService) : base(httpContextAccessor, userService)
        {
            _tribeService = tribeService;
            _notificationService = notificationService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] TribeDTO dto)
        {
            var result = await _tribeService.CreateAsync(this.UserId, dto);

            return Ok(result);
        }

        [HttpPost("{tribeId}/join")]
        public async Task<IActionResult> AcceptTribeInvitationAsync(int tribeId, [FromBody] int? notificationId)
        {
            var status = await _tribeService.AcceptTribeInvitationAsync(this.UserId, tribeId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(notificationId.Value);
            return Ok(status);
        }

        [HttpPost("{tribeId}/reject")]
        public async Task<IActionResult> RejectTribeInvitationAsync(int tribeId, [FromBody] int? notificationId)
        {
            var status = await _tribeService.RejectTribeInvitationAsync(this.UserId, tribeId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(notificationId.Value);
            return Ok(status);
        }
    }
}
