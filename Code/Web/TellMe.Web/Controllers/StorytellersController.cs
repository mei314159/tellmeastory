using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.Web.DAL.Contracts.DTO;
using TellMe.Web.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{
    [Route("api/storytellers")]
    public class StorytellersController : AuthorizedController
    {
        private readonly INotificationService _notificationService;

        public StorytellersController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            INotificationService notificationService) : base(httpContextAccessor, userService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("search/{mode}/skip/{skip}/{fragment}")]
        public async Task<IActionResult> SearchContactsAsync(string fragment, ContactsMode mode, int skip)
        {
            var users = await UserService.SearchContactsAsync(this.UserId, fragment, mode, skip > 0 ? skip : 0);
            return Ok(users);
        }

        [HttpGet("search/{mode}/skip/{skip}")]
        public async Task<IActionResult> SearchContactsAsync(ContactsMode mode, int skip)
        {
            var result = await SearchContactsAsync(null, mode, skip);
            return result;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAsync(string userId)
        {
            var result = await UserService.GetStorytellerAsync(this.UserId, userId);
            return Ok(result);
        }

        [HttpPost("{userId}/add-to-friends")]
        public async Task<IActionResult> AddToFriendsAsync(string userId, [FromBody] int? notificationId)
        {
            var friendshipStatus = await UserService.AddToFriendsAsync(this.UserId, userId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(this.UserId, notificationId.Value);
            return Ok(friendshipStatus);
        }

        [HttpPost("{userId}/unfollow")]
        public async Task<IActionResult> UnfollowAsync(string userId)
        {
            await UserService.UnfollowAsync(this.UserId, userId);
            return Ok();
        }

        [HttpPost("{userId}/reject-friendship")]
        public async Task<IActionResult> RejectFriendshipRequestAsync(string userId, [FromBody] int? notificationId)
        {
            var friendshipStatus = await UserService.RejectFriendshipRequestAsync(this.UserId, userId);
            if (notificationId.HasValue)
                await _notificationService.HandleNotificationAsync(this.UserId, notificationId.Value);
            return Ok(friendshipStatus);
        }

        [HttpPost("request-to-join")]
        public async Task<IActionResult> SendRequestToJoinAsync([FromBody] string email)
        {
            await UserService.SendRequestToJoinAsync(email, this.UserId);
            return Ok();
        }
    }
}