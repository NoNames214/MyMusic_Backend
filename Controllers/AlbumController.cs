using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;
        public AlbumController (IAlbumService albumService)
        {
            _albumService = albumService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<AlbumResponse>>> GetAll()
        {
            var albums = await _albumService.GetAll();
            return Ok(albums);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<AlbumResponse>> GetAlbum (int id)
        {
            var album = await _albumService.GetById(id);
            if (album == null)
            {
                return NotFound();
            }
            return Ok(album);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AlbumResponse>> AddAlbum (AlbumRequest request)
        {
            var albumResponse = await _albumService.AddAlbum(request);
            
            return CreatedAtAction(
                nameof(GetAlbum), 
                new {Id = albumResponse.Id}, 
                albumResponse
            );
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAlbum (int id)
        {
            var result = await _albumService.DeleteAlbum(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAlbum (int id, AlbumRequest request)
        {
            var result = await _albumService.UpdateAlbum(id, request);
            if (!result)
            {
                return NotFound();
            }
            return NoContent(); 

        }

    }
}
