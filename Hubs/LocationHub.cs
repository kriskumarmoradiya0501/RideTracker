using Microsoft.AspNetCore.SignalR;
using RideTracker.Data;
using RideTracker.Models;

namespace RideTracker.Hubs;

public class LocationHub : Hub
{
    private readonly IServiceProvider _serviceProvider;

    public LocationHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task UpdateLocation(double lat, double lng, double speed, string geohash)
    {
        // 1. Broadcast real-time to all clients (Customer View)
        await Clients.Others.SendAsync("ReceiveLocation", lat, lng, speed, geohash, DateTime.UtcNow);

        // 2. Save to Postgres DB in the background
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        db.LocationLogs.Add(new LocationLog 
        { 
            Latitude = lat, Longitude = lng, Speed = speed, Geohash = geohash 
        });
        await db.SaveChangesAsync();
    }

    public async Task SetDriverStatus(bool isLive)
    {
        await Clients.Others.SendAsync("ReceiveDriverStatus", isLive);
    }
}