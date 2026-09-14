namespace MusicApi.IService
{
    public interface IDeviceService
    {
        Task<bool> RegisterDevice(int userId, string fcmToken);
        Task<bool> UnregisterDevice(int userId, string fcmToken);
    }
}
