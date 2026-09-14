using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MusicApi.Request;
using MusicApi.IService;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            await _deviceService.RegisterDevice(userId, request.FcmToken!);

            return NoContent();
        }

        [HttpPost("unregister")]
        public async Task<IActionResult> UnregisterDevice([FromBody] RegisterDeviceRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            await _deviceService.UnregisterDevice(userId, request.FcmToken!);
            return NoContent();
        }
    }
}
