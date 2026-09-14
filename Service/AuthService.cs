using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;
using System.Security.Claims;

namespace MusicApi.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context, 
            IPasswordHasher<User> passwordHasher, IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;

        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null || user.isLocked)
            {
                return null!;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash!,
                request.PassWord!
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return null!;
            }

            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var oldTokens = await _context.RefreshTokens
                .Where(r => r.UserName == user.UserName)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(oldTokens);

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserName = user.UserName,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Role = user.Role
                }
            };
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken!);

            if (principal == null)
                return null!;

            var username = principal.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return null!;

            var savedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r =>
                    r.UserName == username &&
                    r.Token == request.RefreshToken);

            if (savedRefreshToken == null ||
                savedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return null!;
            }

            var user = new User
            {
                Id = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                UserName = username,
                Role = principal.FindFirst(ClaimTypes.Role)!.Value
            };

            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            savedRefreshToken.Token = newRefreshToken;
            savedRefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            try
            {
                var userExisting = await _context.Users
                    .AnyAsync(u => u.UserName == request.UserName);
                if (userExisting)
                {
                    return false;
                }
                var user = new User
                {
                    UserName = request.UserName,
                    Role = "User",
                    PasswordHash = _passwordHasher.HashPassword(null!, request.PassWord!)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<UserResponse> CheckToken(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.isLocked)
            {
                return null!;
            }

            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Avatar = user.Avatar,
                Role = user.Role
            };
        }

    }
}
