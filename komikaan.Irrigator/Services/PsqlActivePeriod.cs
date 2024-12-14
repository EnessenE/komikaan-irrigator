
namespace komikaan.Irrigator.Services
{
    internal class PsqlActivePeriod
    {
        public Guid alert_internal_id { get; set; }
        public string data_origin { get; set; }
        public DateTimeOffset? start_time { get; set; }
        public DateTimeOffset? end_time { get; set; }
    }
}