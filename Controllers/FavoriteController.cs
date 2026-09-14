using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;
using System.Security.Claims;

namespace MusicApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteResponse>>> GetAll()
        {
            var favorites = await _favoriteService.GetAll();
            return Ok(favorites);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<FavoriteResponse>>> GetByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var favorites = await _favoriteService.GetByUserId(id);
            return Ok(favorites);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFavorite(FavoriteRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _favoriteService.DeleteFavorite(id, request.SongId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost] 
        public async Task<ActionResult<FavoriteResponse>> AddFavorite(FavoriteRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var favorite = await _favoriteService.AddFavorite(request, id);
            return CreatedAtAction(
                nameof(GetByUserId), 
                null,
                favorite
            );
        }
    }
}
