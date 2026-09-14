using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;

namespace MusicApi.Service
{
    public class DeviceService : IDeviceService
    {
        private readonly AppDbContext _context;
        public DeviceService (AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterDevice(int userId, string fcmToken)
        {
            var deviceExisting = await _context.UserDevices
                .FirstOrDefaultAsync(ud => ud.FcmToken == fcmToken);

            if (deviceExisting == null)
            {
                _context.UserDevices.Add(new UserDevice
                {
                    UserId = userId,
                    FcmToken = fcmToken,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                deviceExisting.UserId = userId;
                deviceExisting.CreatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnregisterDevice(int userId, string fcmToken)
        {
            var existing = await _context.UserDevices
                .FirstOrDefaultAsync(ud => ud.FcmToken == fcmToken && ud.UserId == userId);

            if(existing == null)
            {
                return false;
            }
            _context.UserDevices.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
