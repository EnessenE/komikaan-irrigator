namespace komikaan.Irrigator.Models
{
    public class RealTimeFeed
    {
        public string Url { get; set; }
        public string SupplierConfigurationName { get; set; }
        public bool Enabled { get; set; }
        public TimeSpan PollingRate { get; set; }
        public DateTimeOffset LastAttempt { get; set; }
        public string? Header { get; set; }
        public string? HeaderSecret { get; set; }
    }
}
