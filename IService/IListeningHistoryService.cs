using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface IListeningHistoryService
    {
        Task<List<ListeningHistoryResponse>> GetByUser(int userId);
        Task<bool> DeleteHistory(int id, int userId);
        Task<bool> DeleteAll(int userId);
        Task<ListeningHistoryResponse> AddHistory(ListeningHistoryRequest request, int userId);
    }
}
