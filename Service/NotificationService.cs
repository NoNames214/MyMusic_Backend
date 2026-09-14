using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Request;
using MusicApi.Response;


namespace MusicApi.Service
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        public NotificationService (AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAll(int userId)
        {
            var notifications = await _context.UserNotifications
                .Where(u => u.UserId == userId)
                .ToListAsync();
            if (!notifications.Any())
            {
                return false;
            }
            _context.UserNotifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotification(int userId, int notificationId)
        {
            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(u => u.UserId == userId && u.NotificationId == notificationId);
            if (userNotification == null)
            {
                return false;
            }
            _context.UserNotifications.Remove(userNotification);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<NotificationResponse>> GetNotifications(int userId)
        {
            var noti = await _context.UserNotifications
                .Where(u => u.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(u => u.Notification!.CreatedAt)
                .Select(u => new NotificationResponse
                {
                    Id = u.Notification!.Id,
                    Title = u.Notification.Title,
                    Body = u.Notification.Body,
                    CreatedAt = u.Notification.CreatedAt,
                    isRead = u.isRead
                }).ToListAsync();
            return noti;
        }

        public async Task<bool> MarkAsRead(int userId, int notificationId)
        {
            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(u => u.UserId == userId && u.NotificationId == notificationId);
            if (userNotification == null)
            {
                return false;
            }
            userNotification.isRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendAll(NotificationRequest request)
        {
            var notification = new Models.Notification
            {
                Title = request.Title,
                Body = request.Body
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                _context.UserNotifications.Add(new UserNotifications
                {
                    UserId = user.Id,
                    NotificationId = notification.Id,
                    isRead = false
                });
            }

            await _context.SaveChangesAsync();

            var tokens = await _context.UserDevices
                .Select(x => x.FcmToken)
                .ToListAsync();

            if (!tokens.Any())
                return false;

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = request.Title,
                    Body = request.Body
                }
            };

            var response = await FirebaseMessaging.DefaultInstance
                .SendEachForMulticastAsync(message);

            if (response.SuccessCount == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendToUser(int userId, NotificationRequest request)
        {
            var notifications = new Models.Notification
            {
                Title = request.Title,
                Body = request.Body
            };
            _context.Notifications.Add(notifications);
            await _context.SaveChangesAsync();

            _context.UserNotifications.Add(new UserNotifications
            {
                UserId = userId,
                NotificationId = notifications.Id,
                isRead = false,
            });

            await _context.SaveChangesAsync();

            var tokens = await _context.UserDevices
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.FcmToken)
                .ToListAsync();

            if (!tokens.Any())
            {
                return false;
            }

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = request.Title,
                    Body = request.Body
                }
            };
            var response = await FirebaseMessaging
                .DefaultInstance.SendEachForMulticastAsync(message);
            if (response.FailureCount > 0)
            {
                return false;
            }
            return true;
        }

        public async Task<int> UnReadCount(int userId)
        {
            var notifications = await _context.UserNotifications
                .Where(u => u.UserId == userId && u.isRead == false)
                .CountAsync();
            if (notifications == 0)
            {
                return 0;
            }
            return notifications;
        }
    }
}
