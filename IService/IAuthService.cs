using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterRequest request);
        Task<LoginResponse> Login (LoginRequest request);
        Task<RefreshTokenResponse> RefreshToken (RefreshTokenRequest request);
        Task<UserResponse> CheckToken(int userId);

    }
}
