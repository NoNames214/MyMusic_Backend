using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public FavoriteService(AppDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        public async Task<List<FavoriteResponse>> GetAll()
        {
            return await _context.Favorites
                .AsNoTracking()
                .Where(f => !f.isDeleted)
                .Select(f => new FavoriteResponse
                {
                    Id = f.Id,
                    SongId = f.SongId,
                    UserId = f.UserId,  
                    Artist = f.Song!.Artist,
                    Title = f.Song.Title,
                    Image = f.Song.Image,
                    Counter = f.Song.Counter,
                    UserName = f.User!.UserName,
                    LikeAt = f.LikeAt!.Value,
                }).ToListAsync();
        }

        public async Task<List<FavoriteResponse>> GetByUserId(int userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId && !f.isDeleted)
                .AsNoTracking()
                .Select(f => new FavoriteResponse
                {
                    Id = f.Id,
                    SongId = f.SongId,
                    UserId = f.UserId,
                    Artist = f.Song!.Artist,
                    Title = f.Song.Title,
                    Image = f.Song.Image,
                    Counter = f.Song.Counter,
                    UserName = f.User!.UserName,
                    LikeAt = f.LikeAt!.Value,
                }).ToListAsync();
            return favorites;
        }

        public async Task<FavoriteResponse> AddFavorite(FavoriteRequest request, int userId)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == request.SongId && !s.isDeleted);
            if (song == null)
            {
                throw new Exception("Song not found.");
            }
            var existingFavorite = await _context.Favorites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == request.SongId);
  
            if (existingFavorite != null && !existingFavorite.isDeleted)
            {
                throw new Exception("Song is already favorited.");
            }

            else if (existingFavorite != null && existingFavorite.isDeleted)
            {
                existingFavorite.isDeleted = false;
                existingFavorite.LikeAt = DateTime.UtcNow;
                song.Counter++;
                await _context.SaveChangesAsync();
                return await _context.Favorites
                    .Where(f => f.Id == existingFavorite.Id)
                    .Select(f => new FavoriteResponse
                    {
                        Id = f.Id,
                        UserId = f.UserId,
                        SongId = f.SongId,
                        Artist = f.Song!.Artist,
                        Title = f.Song.Title,
                        Image = f.Song.Image,
                        Counter = f.Song.Counter,
                        UserName = f.User!.UserName,
                        LikeAt = f.LikeAt!.Value
                     }).FirstAsync();
            }
            else
            {
                var newFavorite = new Favorite
                {
                    UserId = userId,
                    SongId = request.SongId,
                    LikeAt = DateTime.UtcNow,
                    isDeleted = false
                };
                song.Counter++;
                _context.Favorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                await _distributedCache.RemoveAsync("trending_songs");
                return await _context.Favorites
                    .Where(f => f.Id == newFavorite.Id)
                    .Select(f => new FavoriteResponse
                    {
                        Id = f.Id,
                        UserId = f.UserId,
                        SongId = f.SongId,
                        Artist = f.Song!.Artist,
                        Title = f.Song.Title,
                        Image = f.Song.Image,
                        Counter = f.Song.Counter,
                        UserName = f.User!.UserName,
                        LikeAt = f.LikeAt!.Value
                    }).FirstAsync();
            }
        }

        public async Task<bool> DeleteFavorite(int userId, int songId)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == songId && !s.isDeleted);
            if (song == null)
            {
                throw new Exception("Song not found.");
            }
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            if (favorite == null || favorite.isDeleted)
            {
                return false;
            }
            if (song.Counter > 0)
            {
                song.Counter--;
            }
            favorite.isDeleted = true;
            favorite.DeleteAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _distributedCache.RemoveAsync("trending_songs");
            return true;
        }
    }
}
