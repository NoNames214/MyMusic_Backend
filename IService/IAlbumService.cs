using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IAlbumService
    {
        Task<List<AlbumResponse>> GetAll();

        Task<AlbumResponse> GetById(int id);

        Task<bool> DeleteAlbum(int id);

        Task<AlbumResponse> AddAlbum(AlbumRequest request);

        Task<bool> UpdateAlbum(int id, AlbumRequest request);
    }
}
