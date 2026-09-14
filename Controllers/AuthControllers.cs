using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;
using MusicApi.Request;
using System.Security.Claims;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.Register(request);

            if (!result)
            {
                return BadRequest("Username already exists");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshToken(request);

            if (result == null)
                return Unauthorized();

            return NoContent();
        }

        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> CheckToken()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _authService.CheckToken(userId);


            if (user == null)
            {
                return Unauthorized();  
            }

            return NoContent();

        }
    }
}
