using MusicApi.Page;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Service
{
    public interface ISongService
    {
        Task<List<SongResponse>> GetAll(PageResult @params);

        Task<SongResponse?> GetById(int id);
        Task<List<SongResponse>> GetByAlbumId (int albumId);

        Task<SongResponse> AddSong(SongRequest request);

        Task<bool> UpdateSong(int id, SongRequest request);

        Task<bool> Delete(int id);

        Task<List<SongResponse>> SearchSong(string keyword);

        Task<List<SongResponse>> GetTopTrending();
    }
}