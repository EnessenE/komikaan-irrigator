namespace komikaan.Irrigator.Models
{
    internal class PsqlPositionUpdate
    {
        public string id { get; set; }

        public string data_origin { get; set; }
        public Guid internal_id { get; set; }
        public DateTimeOffset last_updated { get; set; }
        public string? trip_id { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? stop_id { get; set; }
        public string current_status { get; set; }
        public DateTimeOffset? measurement_time { get; set; }
        public string occupancy_status { get; set; }
        public string congestion_level { get; set; }
        public int? occupancy_percentage { get; set; }
    }
}