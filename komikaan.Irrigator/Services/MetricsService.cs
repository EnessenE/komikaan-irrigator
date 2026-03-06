using System.Diagnostics.Metrics;

namespace komikaan.Irrigator.Services
{
    /// <summary>
    /// Manages custom metrics for the GTFS Realtime Irrigator application.
    /// </summary>
    public static class MetricsService
    {
        private static readonly Meter _meter = new("komikaan.Irrigator", "1.0.0");

        // Counters for tracking downloads
        public static readonly Counter<int> FeedDownloadCounter = _meter.CreateCounter<int>(
            name: "gtfs_feed_downloads_total",
            unit: "{downloads}",
            description: "Total number of successful GTFS feed downloads");

        public static readonly Counter<int> FeedDownloadFailureCounter = _meter.CreateCounter<int>(
            name: "gtfs_feed_download_failures_total",
            unit: "{failures}",
            description: "Total number of failed GTFS feed downloads");

        // Histogram for tracking download duration
        public static readonly Histogram<double> FeedDownloadDurationMs = _meter.CreateHistogram<double>(
            name: "gtfs_feed_download_duration_ms",
            unit: "ms",
            description: "Duration of GTFS feed downloads in milliseconds");

        // Counters for tracking entities
        public static readonly Counter<int> EntitiesProcessedCounter = _meter.CreateCounter<int>(
            name: "gtfs_entities_processed_total",
            unit: "{entities}",
            description: "Total number of entities processed from GTFS feeds");

        public static readonly Counter<int> TripUpdatesCounter = _meter.CreateCounter<int>(
            name: "gtfs_trip_updates_total",
            unit: "{updates}",
            description: "Total number of trip updates processed");

        public static readonly Counter<int> VehicleUpdatesCounter = _meter.CreateCounter<int>(
            name: "gtfs_vehicle_updates_total",
            unit: "{updates}",
            description: "Total number of vehicle updates processed");

        public static readonly Counter<int> AlertsCounter = _meter.CreateCounter<int>(
            name: "gtfs_alerts_total",
            unit: "{alerts}",
            description: "Total number of alerts processed");

        // Gauge for tracking current process status
        public static readonly UpDownCounter<int> ActiveFeedsCounter = _meter.CreateUpDownCounter<int>(
            name: "gtfs_active_feeds",
            unit: "{feeds}",
            description: "Current number of active feeds being processed");

        public static Meter GetMeter()
        {
            return _meter;
        }
    }
}
