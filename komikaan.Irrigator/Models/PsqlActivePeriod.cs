namespace komikaan.Irrigator.Models
{
    internal class PsqlActivePeriod
    {
        public string id { get; set; }
        public string data_origin { get; set; }
        public DateTimeOffset? start_time { get; set; }
        public DateTimeOffset? end_time { get; set; }
    }
}