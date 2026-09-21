using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IUserService
    {
        Task<UserResponse> GetProfile(int id);

        Task<bool> UpdateUser (int id, UserRequest request);

        Task<bool> DeleteUser (int id);

        Task<bool> ChangePassword(int id, ChangePasswordRequest request);
    }
}
