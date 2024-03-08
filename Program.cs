using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ParkingDemo.Data;
using ParkingDemo.Hubs;
using ParkingDemo.Models;
using ParkingDemo.Services;
using Serilog;

namespace ParkingDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddCors();

        builder.Services.AddDbContextPool<AppDBContext>(optionsBuilder =>
            optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("ProductionDB"), options =>
                options.EnableRetryOnFailure(maxRetryCount: 7)
            ));
        builder.Services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;

            options.User.RequireUniqueEmail = false;
        })
                   .AddEntityFrameworkStores<AppDBContext>()
                   .AddDefaultTokenProviders();

        builder.Services.AddAuthentication().AddCookie();

        builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
        builder.Services.AddSingleton<AppRepository>();

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<MessageQueueService>();
        builder.Services.AddHostedService<HeartbeatService>();

        var logger = new LoggerConfiguration().WriteTo
            .File("./logs/logs.txt",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Logging.AddSerilog(logger, true);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.MapHub<StatusHub>("/hub/status");

        app.MapHub<StateHub>("/hub/state");

        app.Run();
    }
}

 