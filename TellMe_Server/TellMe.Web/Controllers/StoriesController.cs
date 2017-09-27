using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.Web.Controllers
{
    [Route("api/stories")]
    public class StoriesController : AuthorizedController
    {
        private readonly IStoryService _storyService;
        public StoriesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IStoryService storyService) : base(httpContextAccessor, userService)
        {
            _storyService = storyService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestStoryAsync([FromBody] StoryRequestDTO dto)
        {
            await _storyService.RequestStoryAsync(this.UserId, dto);

            return Ok();
        }

        // [HttpGet("")]
        // public async Task<IActionResult> GetContactsAsync()
        // {
        //     var contacts = await _contactsService.GetAllAsync(this.UserId);
        //     return Ok(contacts);
        // }
    }
}
