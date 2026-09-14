using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using System.Security.Claims;


namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController (IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var user = await _userService.GetProfile(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var user = await _userService.UpdateUser(id, request);
            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var user = await _userService.DeleteUser(id);
            return NoContent(); 
        }

    }
}
