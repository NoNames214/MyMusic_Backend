using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Service
{
    public class AlbumService : IAlbumService
    {
        private readonly AppDbContext _context;
        public AlbumService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AlbumResponse> AddAlbum(AlbumRequest request)
        {
            var newAlbum = new Album
            {
                Title = request.Title,
                Artist = request.Artist,
                Image = request.Image,
            };
            _context.Albums.Add(newAlbum);
            await _context.SaveChangesAsync();
            var albumResponse = new AlbumResponse
            {
                Id = newAlbum.Id,
                Title = newAlbum.Title,
                Artist = newAlbum.Artist,
                Image = newAlbum.Image,
                Songs = newAlbum.Songs?.Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration
                }).ToList()
            };
            return albumResponse;
        }

        public async Task<bool> DeleteAlbum(int id)
        {
            var album = await _context.Albums
                .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return false;
            }
            album.isDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AlbumResponse>> GetAll()
        {
            return await _context.Albums
                .AsNoTracking()
                .Select(a => new AlbumResponse
                {
                    Id = a.Id,
                    Title = a.Title,
                    Artist = a.Artist,
                    Image = a.Image,
                    Songs = a.Songs!.Select(s => new SongResponse
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Artist = s.Artist,
                        Duration = s.Duration
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<AlbumResponse> GetById(int id)
        {
            var album =  await _context.Albums
                .Where(a => a.Id == id)
                .AsNoTracking()
                .Select(a => new AlbumResponse
                {
                    Id = a.Id,
                    Title = a.Title,
                    Artist = a.Artist,
                    Image = a.Image,
                    Songs = a.Songs!.Select(s => new SongResponse
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Artist = s.Artist,
                        Duration = s.Duration
                    }).ToList()
                }).FirstOrDefaultAsync();
            return album!;
        }

        public async Task<bool> UpdateAlbum(int id, AlbumRequest request)
        {
            var albumDb = await _context.Albums
                .FirstOrDefaultAsync(a => a.Id == id);
            if (albumDb == null)
            {
                throw new Exception("Album not found");
            }
            albumDb.Title = request.Title;
            albumDb.Artist = request.Artist;
            albumDb.Image = request.Image;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
