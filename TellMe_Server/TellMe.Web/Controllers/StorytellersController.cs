using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{

    [Route("api/storytellers")]
    public class StorytellersController : AuthorizedController
    {
        private INotificationService _notificationService;
        public StorytellersController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            INotificationService notificationService) : base(httpContextAccessor, userService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("search/{fragment}")]
        public async Task<IActionResult> SearchAsync(string fragment)
        {
            var users = await UserService.SearchAsync(this.UserId, fragment);
            return Ok(users);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await UserService.GetAllFriendsAsync(this.UserId);
            return Ok(users);
        }

        [HttpPost("{userId}/add-to-friends")]
        public async Task<IActionResult> AddToFriendsAsync(string userId, [FromBody] int? notificationId)
        {
            var friendshipStatus = await UserService.AddToFriendsAsync(this.UserId, userId);
            await _notificationService.HandleNotificationAsync(notificationId.Value);
            return Ok(friendshipStatus);
        }

        [HttpPost("{userId}/reject-friendship")]
        public async Task<IActionResult> RejectFriendshipRequestAsync(string userId, [FromBody] int? notificationId)
        {
            var friendshipStatus = await UserService.RejectFriendshipRequestAsync(this.UserId, userId);
            await _notificationService.HandleNotificationAsync(notificationId.Value);
            return Ok(friendshipStatus);
        }

        [HttpPost("request-to-join")]
        public async Task<IActionResult> SendRequestToJoinAsync(string email)
        {
            return Ok();
        }
    }
}
