using System.Data;
using System.Text.Json;
using GTFS.Entities;
using Microsoft.EntityFrameworkCore;
using Route = GTFS.Entities.Route;

namespace komikaan.Irrigator.Contexts;

internal class GTFSContext : DbContext
{

    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<CalendarDate> CalendarDates { get; set; }
    public DbSet<Frequency> Frequencies { get; set; }
    public DbSet<StopTime> StopTimes { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<Shape> Shapes { get; set; }

    public GTFSContext(DbContextOptions<GTFSContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.Entity<Frequency>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.TripId,
            item.StartTime,
            item.EndTime
        });
        modelBuilder.Entity<Shape>().HasKey(shape =>
        new
        {
            shape.DataOrigin,
            shape.Id,
            shape.Sequence
        });
        modelBuilder.Entity<Stop>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.InternalId,
            item.Id
        });
        modelBuilder.Entity<Agency>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

        modelBuilder.Entity<Calendar>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.ServiceId
        });

        modelBuilder.Entity<Stop>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

        modelBuilder.Entity<Pathway>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

        modelBuilder.Entity<Route>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

        modelBuilder.Entity<StopTime>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.TripId,
            item.StopId,
            item.StopSequence
        });

        modelBuilder.Entity<Transfer>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

        modelBuilder.Entity<CalendarDate>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Date,
            item.ServiceId
        });

        modelBuilder.Entity<Trip>().HasKey(item =>
        new
        {
            item.DataOrigin,
            item.Id
        });

    }

    protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw");
}