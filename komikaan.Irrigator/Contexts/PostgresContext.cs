using GTFS;
using GTFS.Entities;
using komikaan.Irrigator.Interfaces;
using Z.BulkOperations;

namespace komikaan.Irrigator.Contexts;

internal class PostgresContext : IDataContext
{
    private readonly ILogger<PostgresContext> _logger;
    private readonly GTFSContext _gtfsContext;

    public PostgresContext(ILogger<PostgresContext> logger, GTFSContext gtfsContext)
    {
        _logger = logger;
        _gtfsContext = gtfsContext;
    }

    public async Task ImportAsync(GTFSFeed feed)
    {
        _gtfsContext.Agencies.BulkInsert(feed.Agencies, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Routes.BulkInsert(feed.Routes, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Trips.BulkInsert(feed.Trips, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Stops.BulkInsert(feed.Stops, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Calendars.BulkInsert(feed.Calendars, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.CalendarDates.BulkInsert(feed.CalendarDates, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Frequencies.BulkInsert(feed.Frequencies, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.StopTimes.BulkInsert(feed.StopTimes, operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;

        });
        _gtfsContext.Shapes.BulkInsert(feed.Shapes.ToList(), operation =>
        {
            operation.InsertIfNotExists = true;
            operation.MergeKeepIdentity = true;
            operation.AllowDuplicateKeys = true;
        });

        await _gtfsContext.SaveChangesAsync();
        _logger.LogInformation("Done with import.");
    }
}