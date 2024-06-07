using GTFS;

namespace komikaan.Irrigator.Interfaces;

public interface IDataContext
{
    public Task ImportAsync(GTFSFeed feed);
}