namespace komikaan.Irrigator.Models
{
    public class PsqlTripUpdate
    {
        public string Id { get; set; }
        public string DataOrigin { get; set; }
        public Guid InternalId { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public int? Delay { get; set; }
        public DateTimeOffset? MeasurementTime { get; set; }
    }

}
