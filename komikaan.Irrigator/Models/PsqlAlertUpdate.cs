using TransitRealtime;

namespace komikaan.Irrigator.Models
{
    internal class PsqlAlertUpdate
    {
        public string DataOrigin { get; set; }
        public string Id { get; set; }
        public Guid InternalId { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string? AgencyId { get; set; }
        public string? RouteID { get; set; }
        public string? TripId { get; set; }
        public string? StopId { get; set; }
        public Alert.Effect? Effect { get; internal set; }
        public Alert.Cause? Cause { get; internal set; }
        public Alert.SeverityLevel? SeverityLevel { get; internal set; }
        public string? Url { get; internal set; }
        public string? HeaderText { get; internal set; }
        public string? DescriptionText { get; internal set; }
    }
}