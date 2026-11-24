using Microsoft.EntityFrameworkCore;
using RideTracker.Models;

namespace RideTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<LocationLog> LocationLogs { get; set; }
}