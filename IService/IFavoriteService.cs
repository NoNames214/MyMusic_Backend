using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IFavoriteService
    {
        Task<List<FavoriteResponse>> GetAll();
        Task<List<FavoriteResponse>> GetByUserId(int userId);
        Task<FavoriteResponse> AddFavorite(FavoriteRequest request, int userId);
        Task<bool> DeleteFavorite(int userId, int songId);
    }
}
