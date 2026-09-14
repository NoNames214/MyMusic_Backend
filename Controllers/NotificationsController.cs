using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;
using System.Security.Claims;

namespace MusicApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController (INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var notifications = await _notificationService.GetNotifications(id);
            return Ok(notifications);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("send-all")]
        public async Task<IActionResult> SendAll([FromBody] NotificationRequest request)
        {
            var result = await _notificationService.SendAll(request);
            if (!result)
            {
                return BadRequest("User has no registered device.");
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("send-to-user/{userId}")]
        public async Task<IActionResult> SendToUser(int userId, [FromBody] NotificationRequest request)
        {
            var result = await _notificationService.SendToUser(userId, request);
            if (!result)
            {
                return BadRequest("User has no registered device.");
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPatch("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _notificationService.MarkAsRead(id, notificationId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("delete/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _notificationService.DeleteNotification(id, notificationId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _notificationService.DeleteAll(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> UnReadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var count = await _notificationService.UnReadCount(id);
            return Ok(count);
        }
    }
}
