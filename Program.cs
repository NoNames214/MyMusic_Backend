using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicApi.Data;
using MusicApi.IService;
using MusicApi.Models;
using MusicApi.Service;
using System.Text;
using Serilog; 

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/music_api_.log", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

try
{
    Log.Information("STARTING UP MUSIC API SYSTEM...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    var cs = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(
            cs,
            sqlOptions => sqlOptions.CommandTimeout(180)
        );

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var config = builder.Configuration;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();

    builder.Services.AddScoped<ISongService, SongService>();
    builder.Services.AddScoped<IAlbumService, AlbumService>();
    builder.Services.AddScoped<IPlaylistService, PlaylistService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IDeviceService, DeviceService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IFavoriteService, FavoriteService>();
    builder.Services.AddScoped<IListeningHistoryService, ListeningHistoryService>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFlutter", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "MusicApi",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "{your token}",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        options.InstanceName = "MusicApi_";
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (FirebaseApp.DefaultInstance == null)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json");
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(path)
        });
    }

    app.UseCors("AllowFlutter");
    app.UseSwagger();
    app.UseSwaggerUI();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStaticFiles();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API error: Unable to start!");
}
finally
{
    Log.CloseAndFlush();
}