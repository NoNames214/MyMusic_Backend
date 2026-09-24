using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService (AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

            var imagePath = userInDb.Avatar;
            if (request.Avatar != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.Avatar.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatar");
                var filePath = Path.Combine(folder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Avatar.CopyToAsync(stream);

                imagePath = $"/avatar/{fileName}";
            }

            userInDb.UserName = request.UserName;
            userInDb.Avatar = imagePath;
            userInDb.Email = request.Email;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePassword(int id, ChangePasswordRequest request)
        {
            var userInDb = await _context.Users.FindAsync(id);
            if (userInDb == null)
            {
                return false;
            }
            var isValid = _passwordHasher.VerifyHashedPassword(userInDb, 
                userInDb.PasswordHash!, request.CurrentPassword!);

            if (isValid == PasswordVerificationResult.Failed)
            {
                return false;
            }

            userInDb.PasswordHash = _passwordHasher.HashPassword(userInDb,request.NewPassword!);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
