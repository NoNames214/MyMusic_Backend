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
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
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
            try
            {
                _logger.LogInformation("Login attempt for user: {UserName}", request.UserName);
                var result = await _authService.Login(request);

                if (result == null)
                {
                    _logger.LogWarning("Login failed for user: {UserName}", request.UserName);
                    return Unauthorized();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user: {UserName}", request.UserName);
                return StatusCode(500, "An error occurred while processing your request.");
            }
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
