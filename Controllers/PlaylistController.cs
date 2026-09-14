using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;
using System.Security.Claims;


namespace MusicApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistResponse>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _playlistService.GetAll(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PlaylistResponse>> AddPlaylist ([FromForm]PlaylistRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var playlist = await _playlistService.CreatePlaylist(request, id);
            return Created("", playlist);
        }

        [HttpPost("add-song")]
        public async Task<IActionResult> AddSongToPlaylist(PlaylistSongRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _playlistService.AddSongToPlaylist(request, id);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist (int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var UserId))
            {
                return Unauthorized();
            }
            var playlist = await _playlistService.DeletePlaylist(id, UserId);
            return playlist ? NoContent() : NotFound();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllPlaylists()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _playlistService.DeleteAll(id);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}/song/{songId}")]
        public async Task<IActionResult> DeleteSongFromPlaylist(int id, int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var UserId))
            {
                return Unauthorized();
            }
            var result = await _playlistService.DeleteSong(id, UserId, songId);
            return result ? NoContent() : NotFound();
        }

    } 
}
