using GTFS;
using komikaan.Irrigator.Models;

namespace komikaan.Irrigator.Interfaces
{
    public interface ISupplier
    {
        public Task<GTFSFeed> GetFeedAsync();
        public SupplierConfiguration GetConfiguration();
    }
}
