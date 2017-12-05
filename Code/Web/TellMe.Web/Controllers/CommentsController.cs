using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Contracts.DTO;
using TellMe.Web.Extensions;

namespace TellMe.Web.Controllers
{
    [Route("api/stories/{storyId}/comments")]
    public class CommentsController : AuthorizedController
    {
        private readonly ICommentService _commentService;
        public CommentsController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ICommentService commentService) : base(httpContextAccessor, userService)
        {
            _commentService = commentService;
        }

        [HttpGet("older-than/{olderThanTicksUtc}")]
        public async Task<IActionResult> GetAllAsync(int storyId, long olderThanTicksUtc)
        {
            var olderThanUtc = olderThanTicksUtc.GetUtcDateTime();
            var result = await _commentService.GetAllAsync(storyId, this.UserId, olderThanUtc);

            return Ok(result);
        }
        
        [HttpGet("{replyToCommentId}/replies/older-than/{olderThanTicksUtc}")]
        public async Task<IActionResult> GetRepliesAsync(int storyId, long olderThanTicksUtc, int replyToCommentId)
        {
            var olderThanUtc = olderThanTicksUtc.GetUtcDateTime();
            var result = await _commentService.GetAllAsync(storyId, this.UserId, olderThanUtc, replyToCommentId);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddCommentAsync(int storyId, [FromBody] CommentDTO dto)
        {
            if (dto.StoryId != default(int) && storyId != dto.StoryId)
            {
                return BadRequest();
            }

            var result = await _commentService.AddCommentAsync(this.UserId, storyId, dto);

            return Ok(result);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(int storyId, int commentId)
        {
            await _commentService.DeleteCommentAsync(this.UserId, storyId, commentId);

            return Ok();
        }
    }
}
