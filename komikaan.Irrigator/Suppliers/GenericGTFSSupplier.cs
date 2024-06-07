using GTFS;
using komikaan.Irrigator.Interfaces;
using komikaan.Irrigator.Models;

namespace komikaan.Irrigator.Suppliers;

public class GenericGTFSSupplier : ISupplier
{
    private readonly SupplierConfiguration _supplierConfig;

    public GenericGTFSSupplier(SupplierConfiguration supplierConfig)
    {
        _supplierConfig = supplierConfig;
    }

    public Task<GTFSFeed> GetFeedAsync()
    {
        var reader = new GTFSReader<GTFSFeed>(false, _supplierConfig.Name);
        var feed = reader.Read(_supplierConfig.Url);

        foreach (var agency in feed.Agencies)
        {
            Console.WriteLine("An agency found in this data supplier: {0}", agency.Name);
        }
        Console.WriteLine($"Found a feed with {feed.Agencies.Count} agencies");
        return Task.FromResult(feed);
    }

    public SupplierConfiguration GetConfiguration()
    {
        return _supplierConfig;
    }
}