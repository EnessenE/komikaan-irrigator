public class PsqlAlertEntity
{
    public string data_origin { get; set; }
    public string id { get; set; }
    public DateTimeOffset last_updated { get; set; }
    public string? agency_id { get; set; }
    public string? route_id { get; set; }
    public string? trip_id { get; set; }
    public string? stop_id { get; set; }
}