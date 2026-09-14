using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Response;

namespace MusicApi.Service
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteUser(int Id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == Id);
            if (user == null)
            {
                return false;
            }
            user.isDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserResponse>> GetUser()
        {
            var users = await _context.Users
                .Where(u => u.Role != "Admin")
                .AsNoTracking()
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    Role = u.Role
                }).ToListAsync();
            return users;
        }

        public async Task<bool> LockUser(int Id)
        {
            var user = await  _context.Users
                .FirstOrDefaultAsync(u => u.Id == Id && u.Role != "Admin");
            if (user == null)
            {
                return false;
            }
            user.isLocked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockUser(int Id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == Id && u.Role != "Admin");
            if (user == null)
            {
                return false;
            }
            user.isLocked = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<int> UserCount()
        {
            var userCount = _context.Users
                .Where(u => u.Role != "Admin")
                .CountAsync();
            return userCount;
        }
    }
}
