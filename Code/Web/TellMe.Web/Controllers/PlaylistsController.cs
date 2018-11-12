using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.Shared.Contracts.DTO;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.Extensions;

namespace TellMe.Web.Controllers
{
    [Route("api/playlists")]
    public class PlaylistsController:AuthorizedController
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistsController(IHttpContextAccessor httpContextAccessor, IUserService userService, IPlaylistService playlistService) : base(httpContextAccessor, userService)
        {
            _playlistService = playlistService;
        }
        
        [HttpGet("older-than/{olderThanTicksUtc}")]
        public async Task<IActionResult> GetPlaylistsAsync(long olderThanTicksUtc)
        {
            var olderThanUtc = olderThanTicksUtc.GetUtcDateTime();
            var result = await _playlistService.GetAllAsync(this.UserId, olderThanUtc);

            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistAsync(int id)
        {
            var result = await _playlistService.GetAsync(this.UserId, id);

            return Ok(result);
        }
        
        [HttpGet("{id}/stories")]
        public async Task<IActionResult> GetStoriesAsync(int id)
        {
            var result = await _playlistService.GetStoriesAsync(this.UserId, id);

            return Ok(result);
        }
        
        [HttpPost("{playlistId}/share")]
        public async Task<IActionResult> ShareAsync(int playlistId, [FromBody] SharePlaylistDTO dto)
        {
            await _playlistService.ShareAsync(this.UserId, playlistId, dto);
            return Ok();
        }
        
        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] PlaylistDTO dto)
        {
            var result = await _playlistService.SaveAsync(this.UserId, dto);
            return Ok(result);
        }
        
        [HttpPut("")]
        public async Task<IActionResult> UpdateAsync([FromBody] PlaylistDTO dto)
        {
            var result = await _playlistService.SaveAsync(this.UserId, dto);
            return Ok(result);
        }
        
        [HttpDelete("{playlistId}")]
        public async Task<IActionResult> DeleteAsync(int playlistId)
        {
            await _playlistService.DeleteAsync(this.UserId, playlistId);
            return Ok();
        }
    }
}