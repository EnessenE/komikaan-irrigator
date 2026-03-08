using System.Diagnostics.Metrics;
using komikaan.GTFS.Models.RealTime.Enums;
using komikaan.Irrigator.Models;

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
        
        public static readonly Counter<int> MissingIDsCounter = _meter.CreateCounter<int>(
            name: "gtfs_feed_id_missing_total",
            unit: "{entities}",
            description: "Total number of feed ID's that are missing");

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

        // Histogram for tracking processing time
        public static readonly Histogram<double> FeedProcessingDurationMs = _meter.CreateHistogram<double>(
            name: "gtfs_feed_processing_duration_ms",
            unit: "ms",
            description: "Duration of GTFS feed processing (parsing and database operations) in milliseconds");

        // Histogram for tracking database write latency
        public static readonly Histogram<double> DbWriteLatencyMs = _meter.CreateHistogram<double>(
            name: "gtfs_db_write_latency_ms",
            unit: "ms",
            description: "Database write latency for feed data in milliseconds");

        // Gauge for tracking current process status
        public static readonly UpDownCounter<int> ActiveFeedsCounter = _meter.CreateUpDownCounter<int>(
            name: "gtfs_active_feeds",
            unit: "{feeds}",
            description: "Current number of active feeds being processed");

        public static readonly Counter<int> TripScheduleRelationships = _meter.CreateCounter<int>(
            name: "gtfs_trips_schedule_relationships_total",
            unit: "{total}",
            description: "Total number of schedulerelationships processed");

        public static readonly Counter<int> TripStopTimesScheduleRelationships = _meter.CreateCounter<int>(
            name: "gtfs_trips_stop_times_update_schedule_relationships_total",
            unit: "{total}",
            description: "Total number of schedulerelationships processed");

        // Alert-related metrics
        public static readonly Counter<int> AlertEffectsCounter = _meter.CreateCounter<int>(
            name: "gtfs_alerts_effects_total",
            unit: "{effects}",
            description: "Total number of alert effects processed by type");

        public static readonly Counter<int> AlertCausesCounter = _meter.CreateCounter<int>(
            name: "gtfs_alerts_causes_total",
            unit: "{causes}",
            description: "Total number of alert causes processed by type");

        public static readonly Counter<int> AlertSeveritiesCounter = _meter.CreateCounter<int>(
            name: "gtfs_alerts_severities_total",
            unit: "{severities}",
            description: "Total number of alert severities processed by type");

        // Vehicle position metrics
        public static readonly Counter<int> VehicleCurrentStatusesCounter = _meter.CreateCounter<int>(
            name: "gtfs_vehicle_current_statuses_total",
            unit: "{statuses}",
            description: "Total number of vehicle status updates by current status type");

        public static readonly Counter<int> VehicleCongestionLevelsCounter = _meter.CreateCounter<int>(
            name: "gtfs_vehicle_congestion_levels_total",
            unit: "{levels}",
            description: "Total number of vehicle congestion levels processed");

        public static readonly Counter<int> VehicleOccupancyStatusesCounter = _meter.CreateCounter<int>(
            name: "gtfs_vehicle_occupancy_statuses_total",
            unit: "{statuses}",
            description: "Total number of vehicle occupancy statuses processed");

        public static void TripsScheduleRelationShipUpdateCounter(TripScheduleRelationship? key, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("schedulerelationship", key?.ToString() ?? "none"));
            TripScheduleRelationships.Add(value, expandedTags.ToArray());
        }

        public static void TripsStopsScheduleRelationShipUpdateCounter(StopTimeScheduleRelationship key, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("schedulerelationship", key.ToString()));
            TripStopTimesScheduleRelationships.Add(value, expandedTags.ToArray());
        }

        public static void AlertEffectUpdateCounter(Effect? effect, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("effect", effect?.ToString() ?? "unknown"));
            AlertEffectsCounter.Add(value, expandedTags.ToArray());
        }

        public static void AlertCauseUpdateCounter(Cause? cause, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("cause", cause?.ToString() ?? "unknown"));
            AlertCausesCounter.Add(value, expandedTags.ToArray());
        }

        public static void AlertSeverityUpdateCounter(SeverityLevel? severity, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("severity", severity?.ToString() ?? "unknown"));
            AlertSeveritiesCounter.Add(value, expandedTags.ToArray());
        }

        public static void VehicleCurrentStatusUpdateCounter(VehicleStopStatus? status, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("status", status?.ToString() ?? "unknown"));
            VehicleCurrentStatusesCounter.Add(value, expandedTags.ToArray());
        }

        public static void VehicleCongestionLevelUpdateCounter(CongestionLevel? level, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("congestion_level", level?.ToString() ?? "unknown"));
            VehicleCongestionLevelsCounter.Add(value, expandedTags.ToArray());
        }

        public static void VehicleOccupancyStatusUpdateCounter(OccupancyStatus? status, int value, KeyValuePair<string, object?>[] tags)
        {
            var expandedTags = tags.ToList();
            expandedTags.Add(new KeyValuePair<string, object?>("occupancy_status", status?.ToString() ?? "unknown"));
            VehicleOccupancyStatusesCounter.Add(value, expandedTags.ToArray());
        }

        /// <summary>
        /// Creates metric tags from a RealTimeFeed, including both name and suffix.
        /// </summary>
        public static KeyValuePair<string, object?>[] CreateFeedTags(RealTimeFeed feed)
        {
            return new[]
            {
                new KeyValuePair<string, object?>("feed", feed.SupplierConfigurationName),
                new KeyValuePair<string, object?>("type", feed.Type)
            };
        }

    }
}
