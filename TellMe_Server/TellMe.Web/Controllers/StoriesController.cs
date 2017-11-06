using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Contracts.DTO;
using TellMe.Web.DTO;
using TellMe.Web.Extensions;
using System;

namespace TellMe.Web.Controllers
{
    [Route("api/stories")]
    public class StoriesController : AuthorizedController
    {
        private readonly IStorageService _storageService;
        private INotificationService _notificationService;
        private readonly IStoryService _storyService;
        public StoriesController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IStoryService storyService,
            IStorageService storageService,
            INotificationService notificationService) : base(httpContextAccessor, userService)
        {
            _storyService = storyService;
            _storageService = storageService;
            _notificationService = notificationService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestStoryAsync([FromBody] RequestStoryDTO dto)
        {
            var result = await _storyService.RequestStoryAsync(this.UserId, dto.Requests);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> SendStoryAsync([FromBody] SendStoryDTO dto)
        {
            var result = await _storyService.SendStoryAsync(this.UserId, dto);
            if (dto.NotificationId.HasValue)
                await _notificationService.HandleNotificationAsync(this.UserId, dto.NotificationId.Value);
            return Ok(result);
        }

        [HttpGet("skip/{skip}")]
        public async Task<IActionResult> GetStoriesAsync(int skip)
        {
            var result = await _storyService.GetAllAsync(this.UserId, skip < 0 ? 0 : skip);

            return Ok(result);
        }

        [HttpGet("{storyId}/receivers")]
        public async Task<IActionResult> GetStoryReceiversAsync(int storyId)
        {
            var result = await _storyService.GetStoryReceiversAsync(this.UserId, storyId);

            return Ok(result);
        }

        [HttpGet("{userId}/skip/{skip}")]
        public async Task<IActionResult> GetStoriesAsync(string userId, int skip)
        {
            var result = await _storyService.GetAllAsync(userId, this.UserId, skip < 0 ? 0 : skip);

            return Ok(result);
        }

        [HttpGet("tribe/{tribeId}/skip/{skip}")]
        public async Task<IActionResult> GetStoriesAsync(int tribeId, int skip)
        {
            var result = await _storyService.GetAllAsync(tribeId, this.UserId, skip < 0 ? 0 : skip);

            return Ok(result);
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

        [HttpPost("{storyId}/reject-request")]
        public async Task<IActionResult> RejectFriendshipRequestAsync(int storyId, [FromBody] int? notificationId)
        {
            var storyStatus = await _storyService.RejectRequestAsync(this.UserId, storyId);
            await _notificationService.HandleNotificationAsync(this.UserId, notificationId.Value);
            return Ok(storyStatus);
        }
    }
}
