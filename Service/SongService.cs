using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicApi.Data;
using MusicApi.Models;
using MusicApi.Page;
using MusicApi.Request;
using MusicApi.Response;
using System.Text.Json;

namespace MusicApi.Service
{
    public class SongService : ISongService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;

        public SongService(AppDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }
        
        public async Task<List<SongResponse>> GetTrendingSongs ()
        {
            string cacheKey = "trending_songs";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<SongResponse>>(cachedData)!;
            }
            var songsInDb = await _context.Songs
                .AsNoTracking()
                .OrderByDescending(s => s.Counter)
                .Take(5)
                .Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration,
                    Source = s.Source,
                    Counter = s.Counter,
                    Replay = s.Replay,
                    AlbumId = s.AlbumId,
                    Image = s.Image ?? (s.Album != null ? s.Album.Image : "N/A")
                }).ToListAsync();

            var cachedOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            var jsonToCache = JsonSerializer.Serialize(songsInDb);
            await _distributedCache.SetStringAsync(cacheKey, jsonToCache, cachedOptions);
            return songsInDb;
        }

        public async Task<List<SongResponse>> GetAll(PageResult @params)
        {
            var songs = _context.Songs
                .AsNoTracking()
                .Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration,
                    Source = s.Source,
                    Counter = s.Counter,
                    Replay = s.Replay,
                    AlbumId = s.AlbumId,
                    Image = s.Image ?? (s.Album != null ? s.Album.Image : "N/A")
                }).OrderBy(s => s.Id);
            var pagedData = await songs
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();
            return pagedData;
        }

        public async Task<SongResponse?> GetById(int id)
        {
            return await _context.Songs
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration,
                    Source = s.Source,
                    Replay = s.Replay,
                    Counter = s.Counter,
                    AlbumId = s.AlbumId,
                    Image = s.Image ?? (s.Album != null ? s.Album.Image : "N/A")
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
            {
                return false;
            }
            song.isDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SongResponse> AddSong (SongRequest request)
        {
            var newSong = new Song
            {
                Title = request.Title,
                Artist = request.Artist,
                Duration = request.Duration,
                Source = request.Source,
                Image = request.Image,
                AlbumId = request.AlbumId
            };
            
            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();

            var songResponse = new SongResponse
            {
                Id = newSong.Id,
                Title = newSong.Title,
                Artist = newSong.Artist,
                Duration = newSong.Duration,
                Source = newSong.Source,
                Image = newSong.Image,
                AlbumId = newSong.AlbumId
            };
            return songResponse;
        }

        public async Task<bool> UpdateSong (int id, SongRequest request)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
            {
                return false;
            }

            song.Title = request.Title;
            song.Artist = request.Artist;
            song.Duration = request.Duration;
            song.Source = request.Source;
            song.Image = request.Image;
            song.AlbumId = request.AlbumId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<SongResponse>> SearchSong(string keyword)
        {
            keyword = keyword.Trim();
            const string ? collection = "Latin1_General_CI_AI";

            var song = await _context.Songs
                .Where(s =>
                    (s.Title != null && EF.Functions.Collate(s.Title, collection).Contains(keyword)) ||
                    (s.Artist != null && EF.Functions.Collate(s.Artist, collection).Contains(keyword))
                )
                .AsNoTracking()
                .Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration,
                    Source = s.Source,
                    Replay = s.Replay,
                    Counter = s.Counter,
                    AlbumId = s.AlbumId,
                    Image = s.Image ?? (s.Album != null ? s.Album.Image : "N/A")
                })
                .ToListAsync();
            return song;
        }

        public async Task<List<SongResponse>> GetByAlbumId(int albumId)
        {
            var songs = await _context.Songs
                .Where(s => s.AlbumId == albumId)
                .AsNoTracking()
                .Select(s => new SongResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    Duration = s.Duration,
                    Source = s.Source,
                    Counter = s.Counter,
                    Replay = s.Replay,
                    AlbumId = s.AlbumId,
                    Image = s.Image ?? (s.Album != null ? s.Album.Image : "N/A")
                }).OrderBy(s => s.Id)
                .ToListAsync();
            return songs;
        }
    }
}
