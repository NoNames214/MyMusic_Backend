using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IPlaylistService
    {
        public Task<List<PlaylistResponse>> GetAll(int userId);
        public Task<PlaylistResponse> CreatePlaylist(PlaylistRequest request, int UserId);
        public Task<bool> AddSongToPlaylist(PlaylistSongRequest request, int userId);
        public Task<bool> DeletePlaylist(int id, int userId);
        public Task<bool> DeleteAll(int userId);
        public Task<bool> DeleteSong(int id, int userId, int songId);

    }
}
