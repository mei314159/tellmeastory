using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Contracts.DTO;
using TellMe.Web.DTO;
using TellMe.Web.Extensions;

namespace TellMe.Web.Controllers
{
    [Route("api/stories")]
    public class StoriesController : AuthorizedController
    {
        private readonly IStorageService _storageService;
        private readonly IStoryService _storyService;
        public StoriesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IStoryService storyService,
            IStorageService storageService) : base(httpContextAccessor, userService)
        {
            _storyService = storyService;
            _storageService =storageService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestStoryAsync([FromBody] StoryRequestDTO dto)
        {
            await _storyService.RequestStoryAsync(this.UserId, dto);

            return Ok();
        }

        [HttpPost("")]
        public async Task<IActionResult> SendStoryAsync([FromBody] SendStoryDTO dto)
        {
            var result = await _storyService.SendStoryAsync(this.UserId, dto);

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStoriesAsync(string userId)
        {
            var stories = await _storyService.GetAllAsync(this.UserId, userId);
            return Ok(stories);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetStoriesAsync()
        {
            var stories = await _storyService.GetAllAsync(this.UserId);
            return Ok(stories);
        }

        [HttpPost("upload-media")]
        public async Task<IActionResult> UploadMediaAsync(FileInputDTO dto)
        {
            if (dto == null)
                return BadRequest("Argument null");

            if (dto.VideoFile == null || dto.VideoFile.Length == 0)
                return BadRequest("Video is null");

            if (dto.PreviewImageFile == null || dto.PreviewImageFile.Length == 0)
                return BadRequest("Preview is null");

            var videoBlobName = dto.VideoFile.GetFilename();
            var videoFileStream = await dto.VideoFile.GetFileStream();

            var previewImageBlobName = dto.PreviewImageFile.GetFilename();
            var previewImageStream = await dto.PreviewImageFile.GetFileStream();
            var result = await _storageService.UploadAsync(videoFileStream, videoBlobName, previewImageStream, previewImageBlobName);

            return Ok(result);
        }
    }
}
