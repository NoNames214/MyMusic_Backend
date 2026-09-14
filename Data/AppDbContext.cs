using Microsoft.EntityFrameworkCore;
using MusicApi.Models;
using System.Linq.Expressions;

namespace MusicApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<PlaylistSong> PlaylistSong { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpressions(entityType.ClrType));
                }
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<UserDevice>()
                .HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlits)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId);

            modelBuilder.Entity<Favorite>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.SongId, f.UserId })
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites) 
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Song)
                .WithMany(s => s.Favorites) 
                .HasForeignKey(f => f.SongId);

            modelBuilder.Entity<UserNotifications>()
                .HasKey(un => new { un.UserId, un.NotificationId });

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId);

            modelBuilder.Entity<ListeningHistory>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<ListeningHistory>()
                .HasOne(h => h.User)
                .WithMany(u => u.listeningHistories)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListeningHistory>()
                .HasOne(h => h.Song)
                .WithMany(s => s.listeningHistories)
                .HasForeignKey(h => h.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static LambdaExpression ConvertFilterExpressions(Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.isDeleted));
            var falseConstant = Expression.Constant(false);
            var comparison = Expression.Equal(property, falseConstant);
            return Expression.Lambda(comparison, parameter);
        }
    }
}