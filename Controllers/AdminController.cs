using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApi.IService;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("status/users-count")]
        public async Task<IActionResult> UserCount()
        {
            var count = await _adminService.UserCount();
            return Ok(new 
            { 
                userCount = count 
            });
        }

        [HttpGet("users-list")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _adminService.GetUser();
            return Ok(users);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUser(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("users/{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var result = await _adminService.LockUser(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("users/{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var result = await _adminService.UnlockUser(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
