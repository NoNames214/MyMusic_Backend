using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;


namespace MusicApi.Service
{
    public class PlaylistService : IPlaylistService
    {
        private readonly AppDbContext _context;
        public PlaylistService(AppDbContext context)
        {
            _context = context;
        }  

        public async Task<List<PlaylistResponse>> GetAll(int userId)
        {
            return await _context.Playlist
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => new PlaylistResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User != null ? p.User.UserName : "N/A",
                    Songs = p.PlaylistSongs!.Select(ps => new SongResponse
                    {
                        Id = ps.Song!.Id,
                        Title = ps.Song.Title,
                        Artist = ps.Song.Artist,
                        Duration = ps.Song.Duration
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> DeletePlaylist(int id, int userId)
        {
            var playlist = await _context.Playlist
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (playlist == null)
            {
                return false;
            }
            playlist.isDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAll(int userId)
        {
            var playlists = await _context.Playlist
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (playlists.Count == 0)
            {
                return false;
            }

            foreach (var playlist in playlists)
            {
                playlist.isDeleted = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlaylistResponse> CreatePlaylist(PlaylistRequest request, int UserId)
        {
            var imagePath = "";
            if (request.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.ImageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "playlists");
                var filePath = Path.Combine(folder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ImageFile.CopyToAsync(stream);

                imagePath = $"/playlists/{fileName}";
            }

            var newPlaylist = new Playlist
            {
                Title = request.Title,
                Description = request.Description,
                Image = imagePath,
                UserId = UserId,
            };
            _context.Playlist.Add(newPlaylist);
            await _context.SaveChangesAsync();
            return await _context.Playlist
                .Where(p => p.Id == newPlaylist.Id)
                .Select(p => new PlaylistResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Image = p.Image,
                    UserId = p.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UserName = p.User != null ? p.User.UserName : "N/A",
                    Songs = new List<SongResponse>()
                })
                .FirstOrDefaultAsync() ?? throw new Exception();
        }

        public async Task<bool> AddSongToPlaylist(PlaylistSongRequest request, int userId)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == request.SongId);
            if (song == null)
            {
                throw new Exception("Song not found");
            }
            var playlist = await _context.Playlist
                .FirstOrDefaultAsync(p => p.Id == request.PlaylistId && p.UserId == userId);
            if (playlist == null)
            {
                throw new Exception("Playlist not found");
            }

            var existingPlaylist = await _context.PlaylistSong
                .AnyAsync(ps => ps.PlaylistId == request.PlaylistId && ps.SongId == request.SongId);
            if (existingPlaylist)
            {
                throw new Exception("Song already exists in the playlist");
            } 

            var songPlaylist = new PlaylistSong
            {
                PlaylistId = request.PlaylistId,
                SongId = request.SongId
            };
            _context.PlaylistSong.Add(songPlaylist);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteSong(int id, int userId, int songId)
        {
            var playlistSong = await _context.PlaylistSong
                .FirstOrDefaultAsync(ps =>
                    ps.PlaylistId == id &&
                    ps.SongId == songId &&
                    ps.Playlist!.UserId == userId);

            if (playlistSong == null)
                return false;

            _context.PlaylistSong.Remove(playlistSong);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
