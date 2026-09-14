using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IAdminService
    {
        Task<List<UserResponse>> GetUser();
        Task<int> UserCount();
        Task<bool> DeleteUser(int Id);
        Task<bool> UnlockUser(int Id);
        Task<bool> LockUser(int Id);
    }
}
