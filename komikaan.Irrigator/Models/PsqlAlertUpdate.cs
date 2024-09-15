namespace komikaan.Irrigator.Models
{
    internal class PsqlAlertUpdate
    {
        public string data_origin { get; set; }
        public Guid internal_id { get; set; }
        public DateTimeOffset last_updated { get; set; }
        public object agency_id { get; set; }
        public object route_id { get; set; }
        public object trip_id { get; set; }
        public object stop_id { get; set; }
    }
}