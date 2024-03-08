using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkingDemo.Models;

namespace ParkingDemo.Data;

public class AppDBContext : IdentityDbContext<AppUser, AppRole, int>
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<Camera> Cameras { get; set; }
    public DbSet<CameraLog> CameraLogs { get; set; }
    public DbSet<Log> Logs { get; set; }


    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}