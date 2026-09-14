using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.Service
{
    public class ListeningHistoryService : IListeningHistoryService
    {
        private readonly AppDbContext _context;

        public ListeningHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ListeningHistoryResponse>> GetByUser(int userId)
        {
            return await _context.ListeningHistories
                .AsNoTracking()
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.PlayedAt)
                .Take(20)
                .Select(h => new ListeningHistoryResponse
                {
                    Id = h.Id,
                    UserId = h.UserId,
                    SongId = h.SongId,
                    Artist = h.Song!.Artist,
                    Image = h.Song.Image,
                    Title = h.Song.Title,
                    PlayedAt = h.PlayedAt,
                })
                .ToListAsync();
        }

        public async Task<ListeningHistoryResponse> AddHistory(ListeningHistoryRequest request, int userId)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == request.SongId);

            if (song == null)
                throw new Exception("Song not found.");

            var listeningHistory = new ListeningHistory
            {
                UserId = userId,
                SongId = request.SongId,
                PlayedAt = DateTime.UtcNow,
                isDeleted = false
            };

            _context.ListeningHistories.Add(listeningHistory);
            await _context.SaveChangesAsync();
            return new ListeningHistoryResponse
            {
                Id = listeningHistory.Id,
                UserId = userId,
                SongId = song.Id,
                Artist = song.Artist,
                Title = song.Title,
                Image = song.Image,
                PlayedAt = listeningHistory.PlayedAt
            };
        }

        public async Task<bool> DeleteAll(int userId)
        {
            var result = await _context.ListeningHistories
                .Where(h => h.UserId == userId)
                .ToListAsync();

            if (result.Count == 0)
                return false;

            _context.ListeningHistories.RemoveRange(result);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteHistory(int id, int userId)
        {
            var result = await _context.ListeningHistories
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (result == null)
                return false;

            _context.ListeningHistories.Remove(result);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}