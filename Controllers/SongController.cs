using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.Page;
using MusicApi.Request;
using MusicApi.Response;
using MusicApi.Service;

namespace MyMusic.Manager
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ISongService _isongService;
        private readonly ILogger<SongController> _logger;
        public SongController(ISongService isongService, ILogger<SongController> logger)
        {
            _isongService = isongService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<SongResponse>>> GetAll([FromQuery] PageResult @params)
        {
            try
            {
                _logger.LogInformation("Fetching all songs with parameters: {@Params}", @params);
                var song = await _isongService.GetAll(@params);
                return Ok(song);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all songs.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin, User")]
        public async Task<ActionResult<SongResponse>> GetSong(int id)
        {
            var song = await _isongService.GetById(id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }

        [HttpGet("album/{albumId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<SongResponse>>> GetByAlbumId (int albumId)
        {
            var song = await _isongService.GetByAlbumId(albumId);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _isongService.Delete(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task <ActionResult<SongResponse>> AddSongs(SongRequest songDto) 
        {
            var songResponse = await _isongService.AddSong(songDto);
            return CreatedAtAction(
                nameof(GetSong),
                new {Id = songResponse.Id},
                songResponse
             );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSongs (int id, SongRequest request)
        {
            var songResponse = await _isongService.UpdateSong(id, request);
            return NoContent();
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<SongResponse>>> SearchSongs([FromQuery] string keyword)
        {
            var songs = await _isongService.SearchSong(keyword);
            return Ok(songs);
        }

        [HttpGet("Top-trending")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<SongResponse>>> GetTopTrending()
        {
            var songs = await _isongService.GetTopTrending();
            return Ok(songs);
        }
    }
}
