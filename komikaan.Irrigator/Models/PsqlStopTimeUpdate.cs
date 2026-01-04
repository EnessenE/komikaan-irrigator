namespace komikaan.Irrigator.Models
{
    public class PsqlStopTimeUpdate
    {
        public string DataOrigin { get; set; }
        public string Id { get; set; }
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
        public int? StopSequence { get; set; }
        public string? StopId { get; set; }
        public string? TripId { get; set; }
        public int? ArrivalDelay { get; set; }
        public DateTimeOffset? ArrivalTime { get; set; }
        public int? ArrivalUncertainty { get; set; }
        public int? DepartureDelay { get; set; }
        public DateTimeOffset? DepartureTime { get; set; }
        public int? DepartureUncertainty { get; set; }
        public string ScheduleRelationship { get; set; }
    }

}
