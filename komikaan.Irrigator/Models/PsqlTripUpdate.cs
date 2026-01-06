using NpgsqlTypes;

namespace komikaan.Irrigator.Models
{
    public class PsqlTripUpdate
    {
        [PgName("data_origin")]
        public string DataOrigin { get; set; }

        [PgName("internal_id")]
        public Guid InternalId { get; set; }

        [PgName("id")]
        public string Id { get; set; }

        [PgName("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [PgName("delay")]
        public int? Delay { get; set; }

        [PgName("schedule_relationship")]
        public string? ScheduleRelationShip { get; set; }

        [PgName("vehicle_id")]
        public string? VehicleId { get; set; }

        [PgName("vehicle_label")]
        public string? VehicleLabel { get; set; }

        [PgName("vehicle_license_plate")]
        public string? VehicleLicensePlate { get; set; }

        [PgName("vehicle_wheelchair_accessible")]
        public string? VehicleWheelchairAccessible { get; set; }

        [PgName("measurement_time")]
        public DateTimeOffset? MeasurementTime { get; set; }
    }
}
