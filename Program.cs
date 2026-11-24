using Microsoft.EntityFrameworkCore;
using Npgsql;
using RideTracker.Data;
using RideTracker.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add WebSockets support

// Render supplies PostgreSQL credentials through DATABASE_URL. Local development
// automatically uses EF Core's in-memory database when no connection is configured.
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = builder.Configuration["DATABASE_URL"];

var databaseValue = string.IsNullOrWhiteSpace(connString) ? databaseUrl : connString;

if (!string.IsNullOrWhiteSpace(databaseValue)
    && Uri.TryCreate(databaseValue, UriKind.Absolute, out var databaseUri)
    && (databaseUri.Scheme == "postgres" || databaseUri.Scheme == "postgresql"))
{
    var uriUserInfo = databaseUri.UserInfo.Split(':', 2);
    var postgres = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
        Username = Uri.UnescapeDataString(uriUserInfo.ElementAtOrDefault(0) ?? ""),
        Password = Uri.UnescapeDataString(uriUserInfo.ElementAtOrDefault(1) ?? ""),
        Database = databaseUri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require
    };
    connString = postgres.ToString();
}

var useInMemoryDatabase = string.IsNullOrWhiteSpace(connString);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("RideTracker");
    }
    else
    {
        options.UseNpgsql(connString);
    }
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<LocationHub>("/locationHub"); // Map SignalR endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Apply Migrations automatically for demo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        app.Logger.LogInformation(useInMemoryDatabase
            ? "Using the in-memory demo database."
            : "Using the configured PostgreSQL database.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex,
            "Database initialization failed. Set DATABASE_URL or ConnectionStrings__DefaultConnection.");
    }
}

app.Run();