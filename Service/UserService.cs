using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Request;
using MusicApi.Response;
using System.Xml;

namespace MusicApi.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService (AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            user.isDeleted = true;
            user.DeleteAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponse> GetProfile(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Avatar = x.Avatar,
                    Email = x.Email,
                    Role = x.Role,
                })
                .FirstOrDefaultAsync();
            return user!;
                
        }

        public async Task<bool> UpdateUser(int id, UserRequest request)
        {
            var userInDb = await _context.Users.FindAsync(id);
            if (userInDb == null)
            {
                return false;
            }

            userInDb.UserName = request.UserName;
            userInDb.Avatar = request.Avatar;
            userInDb.Email = request.Email;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
