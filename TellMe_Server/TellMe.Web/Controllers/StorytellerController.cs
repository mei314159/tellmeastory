using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{
    [Route("api/storytellers")]
    public class StorytellerController : AuthorizedController
    {
        
        public StorytellerController(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor, userService)
        {
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
        public async Task<IActionResult> AddToFriendsAsync(string userId)
        {
            var friendshipStatus = await UserService.AddToFriendsAsync(this.UserId, userId);
            return Ok(friendshipStatus);
        }
    }
}
