using System;
using System.ComponentModel.DataAnnotations;

namespace RideTracker.Models;

public class LocationLog
{
    [Key]
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public string Geohash { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}