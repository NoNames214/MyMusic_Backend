using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;
using System.Security.Claims;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ListeningHistoryController : ControllerBase
    {
        private readonly IListeningHistoryService _listeningHistoryService;
        public ListeningHistoryController(IListeningHistoryService listeningHistoryService)
        {
            _listeningHistoryService = listeningHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListeningHistoryResponse>>> GetListeningHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var history = await _listeningHistoryService.GetByUser(id);
            return Ok(history);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListeningHistory(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var UserId))
            {
                return Unauthorized();
            }
            var result = await _listeningHistoryService.DeleteHistory(id, UserId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("all")] 
        public async Task<IActionResult> DeleteAllListeningHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _listeningHistoryService.DeleteAll(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ListeningHistoryResponse>> AddListeningHistory(ListeningHistoryRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var history = await _listeningHistoryService.AddHistory(request, id);
            return Created("", history);
        }
    }
}
