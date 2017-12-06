using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;

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

        [HttpGet("{tribeId}")]
        public async Task<IActionResult> GetAsync(int tribeId)
        {
            var result = await _tribeService.GetAsync(this.UserId, tribeId);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] TribeDTO dto)
        {
            var result = await _tribeService.CreateAsync(this.UserId, dto);

            return Ok(result);
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateAsync([FromBody] TribeDTO dto)
        {
            var isTribeCreator = await _tribeService.IsTribeCreatorAsync(this.UserId, dto.Id);
            if (!isTribeCreator)
                return BadRequest("You don't have permissions to update this tribe");

            var result = await _tribeService.UpdateAsync(this.UserId, dto);
            return Ok(result);
        }

        [HttpPost("{tribeId}/join")]
        public async Task<IActionResult> AcceptTribeInvitationAsync(int tribeId, [FromBody] int? notificationId)
        {
            var status = await _tribeService.AcceptTribeInvitationAsync(this.UserId, tribeId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(this.UserId, notificationId.Value);
            return Ok(status);
        }

        [HttpPost("{tribeId}/leave")]
        public async Task<IActionResult> LeaveTribeAsync(int tribeId)
        {
            var isTribeCreator = await _tribeService.IsTribeCreatorAsync(this.UserId, tribeId);
            if (isTribeCreator)
                return BadRequest("Tribe creator  can't leave a tribe");
            await _tribeService.LeaveTribeAsync(this.UserId, tribeId);
            return Ok();
        }

        [HttpPost("{tribeId}/reject")]
        public async Task<IActionResult> RejectTribeInvitationAsync(int tribeId, [FromBody] int? notificationId)
        {
            var status = await _tribeService.RejectTribeInvitationAsync(this.UserId, tribeId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(this.UserId, notificationId.Value);
            return Ok(status);
        }
    }
}
