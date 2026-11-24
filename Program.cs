using Microsoft.EntityFrameworkCore;
using RideTracker.Data;
using RideTracker.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add WebSockets support

// Configure PostgreSQL
var connString = builder.Configuration.GetConnectionString("DefaultConnection") 
                 ?? "Host=localhost;Database=ridetracker;Username=postgres;Password=yourpassword";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<LocationHub>("/locationHub"); // Map SignalR endpoint

// Apply Migrations automatically for demo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Creates DB if not exists
}

app.Run();